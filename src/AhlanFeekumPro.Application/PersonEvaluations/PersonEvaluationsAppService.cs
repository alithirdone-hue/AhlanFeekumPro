using AhlanFeekumPro.Shared;
using AhlanFeekumPro.UserProfiles;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.PersonEvaluations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PersonEvaluations
{

    [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Default)]
    public abstract class PersonEvaluationsAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<PersonEvaluationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPersonEvaluationRepository _personEvaluationRepository;
        protected PersonEvaluationManager _personEvaluationManager;

        protected IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public PersonEvaluationsAppServiceBase(IPersonEvaluationRepository personEvaluationRepository, PersonEvaluationManager personEvaluationManager, IDistributedCache<PersonEvaluationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _personEvaluationRepository = personEvaluationRepository;
            _personEvaluationManager = personEvaluationManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<PersonEvaluationWithNavigationPropertiesDto>> GetListAsync(GetPersonEvaluationsInput input)
        {
            var totalCount = await _personEvaluationRepository.GetCountAsync(input.FilterText, input.RateMin, input.RateMax, input.Comment, input.EvaluatorId, input.EvaluatedPersonId);
            var items = await _personEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.Comment, input.EvaluatorId, input.EvaluatedPersonId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PersonEvaluationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PersonEvaluationWithNavigationProperties>, List<PersonEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PersonEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PersonEvaluationWithNavigationProperties, PersonEvaluationWithNavigationPropertiesDto>
                (await _personEvaluationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<PersonEvaluationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PersonEvaluation, PersonEvaluationDto>(await _personEvaluationRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AhlanFeekumPro.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AhlanFeekumPro.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _personEvaluationRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Create)]
        public virtual async Task<PersonEvaluationDto> CreateAsync(PersonEvaluationCreateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var personEvaluation = await _personEvaluationManager.CreateAsync(
            input.EvaluatorId, input.EvaluatedPersonId, input.Rate, input.Comment
            );

            return ObjectMapper.Map<PersonEvaluation, PersonEvaluationDto>(personEvaluation);
        }

        [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Edit)]
        public virtual async Task<PersonEvaluationDto> UpdateAsync(Guid id, PersonEvaluationUpdateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var personEvaluation = await _personEvaluationManager.UpdateAsync(
            id,
            input.EvaluatorId, input.EvaluatedPersonId, input.Rate, input.Comment, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PersonEvaluation, PersonEvaluationDto>(personEvaluation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PersonEvaluationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var personEvaluations = await _personEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.Comment, input.EvaluatorId, input.EvaluatedPersonId);
            var items = personEvaluations.Select(item => new
            {
                Rate = item.PersonEvaluation.Rate,
                Comment = item.PersonEvaluation.Comment,

                Evaluator = item.Evaluator?.Name,
                EvaluatedPerson = item.EvaluatedPerson?.Name,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PersonEvaluations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> personevaluationIds)
        {
            await _personEvaluationRepository.DeleteManyAsync(personevaluationIds);
        }

        [Authorize(AhlanFeekumProPermissions.PersonEvaluations.Delete)]
        public virtual async Task DeleteAllAsync(GetPersonEvaluationsInput input)
        {
            await _personEvaluationRepository.DeleteAllAsync(input.FilterText, input.RateMin, input.RateMax, input.Comment, input.EvaluatorId, input.EvaluatedPersonId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PersonEvaluationDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new AhlanFeekumPro.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}