using AhlanFeekumPro.Shared;
using AhlanFeekumPro.SiteProperties;
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
using AhlanFeekumPro.PropertyEvaluations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyEvaluations
{

    [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Default)]
    public abstract class PropertyEvaluationsAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<PropertyEvaluationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPropertyEvaluationRepository _propertyEvaluationRepository;
        protected PropertyEvaluationManager _propertyEvaluationManager;

        protected IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> _sitePropertyRepository;

        public PropertyEvaluationsAppServiceBase(IPropertyEvaluationRepository propertyEvaluationRepository, PropertyEvaluationManager propertyEvaluationManager, IDistributedCache<PropertyEvaluationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> sitePropertyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _propertyEvaluationRepository = propertyEvaluationRepository;
            _propertyEvaluationManager = propertyEvaluationManager; _userProfileRepository = userProfileRepository;
            _sitePropertyRepository = sitePropertyRepository;

        }

        public virtual async Task<PagedResultDto<PropertyEvaluationWithNavigationPropertiesDto>> GetListAsync(GetPropertyEvaluationsInput input)
        {
            var totalCount = await _propertyEvaluationRepository.GetCountAsync(input.FilterText, input.CleanlinessMin, input.CleanlinessMax, input.PriceAndValueMin, input.PriceAndValueMax, input.LocationMin, input.LocationMax, input.AccuracyMin, input.AccuracyMax, input.AttitudeMin, input.AttitudeMax, input.RatingComment, input.UserProfileId, input.SitePropertyId);
            var items = await _propertyEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.CleanlinessMin, input.CleanlinessMax, input.PriceAndValueMin, input.PriceAndValueMax, input.LocationMin, input.LocationMax, input.AccuracyMin, input.AccuracyMax, input.AttitudeMin, input.AttitudeMax, input.RatingComment, input.UserProfileId, input.SitePropertyId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PropertyEvaluationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PropertyEvaluationWithNavigationProperties>, List<PropertyEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PropertyEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyEvaluationWithNavigationProperties, PropertyEvaluationWithNavigationPropertiesDto>
                (await _propertyEvaluationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<PropertyEvaluationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyEvaluation, PropertyEvaluationDto>(await _propertyEvaluationRepository.GetAsync(id));
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetSitePropertyLookupAsync(LookupRequestDto input)
        {
            var query = (await _sitePropertyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.PropertyTitle != null &&
                         x.PropertyTitle.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AhlanFeekumPro.SiteProperties.SiteProperty>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AhlanFeekumPro.SiteProperties.SiteProperty>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _propertyEvaluationRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Create)]
        public virtual async Task<PropertyEvaluationDto> CreateAsync(PropertyEvaluationCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyEvaluation = await _propertyEvaluationManager.CreateAsync(
            input.UserProfileId, input.SitePropertyId, input.Cleanliness, input.PriceAndValue, input.Location, input.Accuracy, input.Attitude, input.RatingComment
            );

            return ObjectMapper.Map<PropertyEvaluation, PropertyEvaluationDto>(propertyEvaluation);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Edit)]
        public virtual async Task<PropertyEvaluationDto> UpdateAsync(Guid id, PropertyEvaluationUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyEvaluation = await _propertyEvaluationManager.UpdateAsync(
            id,
            input.UserProfileId, input.SitePropertyId, input.Cleanliness, input.PriceAndValue, input.Location, input.Accuracy, input.Attitude, input.RatingComment, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PropertyEvaluation, PropertyEvaluationDto>(propertyEvaluation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyEvaluationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var propertyEvaluations = await _propertyEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.CleanlinessMin, input.CleanlinessMax, input.PriceAndValueMin, input.PriceAndValueMax, input.LocationMin, input.LocationMax, input.AccuracyMin, input.AccuracyMax, input.AttitudeMin, input.AttitudeMax, input.RatingComment, input.UserProfileId, input.SitePropertyId);
            var items = propertyEvaluations.Select(item => new
            {
                Cleanliness = item.PropertyEvaluation.Cleanliness,
                PriceAndValue = item.PropertyEvaluation.PriceAndValue,
                Location = item.PropertyEvaluation.Location,
                Accuracy = item.PropertyEvaluation.Accuracy,
                Attitude = item.PropertyEvaluation.Attitude,
                RatingComment = item.PropertyEvaluation.RatingComment,

                UserProfile = item.UserProfile?.Name,
                SiteProperty = item.SiteProperty?.PropertyTitle,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PropertyEvaluations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> propertyevaluationIds)
        {
            await _propertyEvaluationRepository.DeleteManyAsync(propertyevaluationIds);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyEvaluations.Delete)]
        public virtual async Task DeleteAllAsync(GetPropertyEvaluationsInput input)
        {
            await _propertyEvaluationRepository.DeleteAllAsync(input.FilterText, input.CleanlinessMin, input.CleanlinessMax, input.PriceAndValueMin, input.PriceAndValueMax, input.LocationMin, input.LocationMax, input.AccuracyMin, input.AccuracyMax, input.AttitudeMin, input.AttitudeMax, input.RatingComment, input.UserProfileId, input.SitePropertyId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PropertyEvaluationDownloadTokenCacheItem { Token = token },
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