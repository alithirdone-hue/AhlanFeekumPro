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
using AhlanFeekumPro.Governorates;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.Governorates
{

    [Authorize(AhlanFeekumProPermissions.Governorates.Default)]
    public abstract class GovernoratesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<GovernorateDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IGovernorateRepository _governorateRepository;
        protected GovernorateManager _governorateManager;

        public GovernoratesAppServiceBase(IGovernorateRepository governorateRepository, GovernorateManager governorateManager, IDistributedCache<GovernorateDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _governorateRepository = governorateRepository;
            _governorateManager = governorateManager;

        }

        public virtual async Task<PagedResultDto<GovernorateDto>> GetListAsync(GetGovernoratesInput input)
        {
            var totalCount = await _governorateRepository.GetCountAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _governorateRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<GovernorateDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Governorate>, List<GovernorateDto>>(items)
            };
        }

        public virtual async Task<GovernorateDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Governorate, GovernorateDto>(await _governorateRepository.GetAsync(id));
        }

        [Authorize(AhlanFeekumProPermissions.Governorates.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _governorateRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.Governorates.Create)]
        public virtual async Task<GovernorateDto> CreateAsync(GovernorateCreateDto input)
        {

            var governorate = await _governorateManager.CreateAsync(
            input.Title, input.Order, input.IsActive
            );

            return ObjectMapper.Map<Governorate, GovernorateDto>(governorate);
        }

        [Authorize(AhlanFeekumProPermissions.Governorates.Edit)]
        public virtual async Task<GovernorateDto> UpdateAsync(Guid id, GovernorateUpdateDto input)
        {

            var governorate = await _governorateManager.UpdateAsync(
            id,
            input.Title, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Governorate, GovernorateDto>(governorate);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(GovernorateExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _governorateRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Governorate>, List<GovernorateExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Governorates.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.Governorates.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> governorateIds)
        {
            await _governorateRepository.DeleteManyAsync(governorateIds);
        }

        [Authorize(AhlanFeekumProPermissions.Governorates.Delete)]
        public virtual async Task DeleteAllAsync(GetGovernoratesInput input)
        {
            await _governorateRepository.DeleteAllAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new GovernorateDownloadTokenCacheItem { Token = token },
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