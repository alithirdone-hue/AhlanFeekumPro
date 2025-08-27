using AhlanFeekumPro.Shared;
using AhlanFeekumPro.SiteProperties;
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
using AhlanFeekumPro.PropertyMedias;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyMedias
{

    [Authorize(AhlanFeekumProPermissions.PropertyMedias.Default)]
    public abstract class PropertyMediasAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<PropertyMediaDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPropertyMediaRepository _propertyMediaRepository;
        protected PropertyMediaManager _propertyMediaManager;

        protected IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> _sitePropertyRepository;

        public PropertyMediasAppServiceBase(IPropertyMediaRepository propertyMediaRepository, PropertyMediaManager propertyMediaManager, IDistributedCache<PropertyMediaDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> sitePropertyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _propertyMediaRepository = propertyMediaRepository;
            _propertyMediaManager = propertyMediaManager; _sitePropertyRepository = sitePropertyRepository;

        }

        public virtual async Task<PagedResultDto<PropertyMediaWithNavigationPropertiesDto>> GetListAsync(GetPropertyMediasInput input)
        {
            var totalCount = await _propertyMediaRepository.GetCountAsync(input.FilterText, input.Image, input.OrderMin, input.OrderMax, input.isActive, input.SitePropertyId);
            var items = await _propertyMediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Image, input.OrderMin, input.OrderMax, input.isActive, input.SitePropertyId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PropertyMediaWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PropertyMediaWithNavigationProperties>, List<PropertyMediaWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PropertyMediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyMediaWithNavigationProperties, PropertyMediaWithNavigationPropertiesDto>
                (await _propertyMediaRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<PropertyMediaDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyMedia, PropertyMediaDto>(await _propertyMediaRepository.GetAsync(id));
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

        [Authorize(AhlanFeekumProPermissions.PropertyMedias.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _propertyMediaRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyMedias.Create)]
        public virtual async Task<PropertyMediaDto> CreateAsync(PropertyMediaCreateDto input)
        {
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyMedia = await _propertyMediaManager.CreateAsync(
            input.SitePropertyId, input.Image, input.Order, input.isActive
            );

            return ObjectMapper.Map<PropertyMedia, PropertyMediaDto>(propertyMedia);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyMedias.Edit)]
        public virtual async Task<PropertyMediaDto> UpdateAsync(Guid id, PropertyMediaUpdateDto input)
        {
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyMedia = await _propertyMediaManager.UpdateAsync(
            id,
            input.SitePropertyId, input.Image, input.Order, input.isActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PropertyMedia, PropertyMediaDto>(propertyMedia);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyMediaExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var propertyMedias = await _propertyMediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Image, input.OrderMin, input.OrderMax, input.isActive, input.SitePropertyId);
            var items = propertyMedias.Select(item => new
            {
                Image = item.PropertyMedia.Image,
                Order = item.PropertyMedia.Order,
                isActive = item.PropertyMedia.isActive,

                SiteProperty = item.SiteProperty?.PropertyTitle,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PropertyMedias.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.PropertyMedias.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> propertymediaIds)
        {
            await _propertyMediaRepository.DeleteManyAsync(propertymediaIds);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyMedias.Delete)]
        public virtual async Task DeleteAllAsync(GetPropertyMediasInput input)
        {
            await _propertyMediaRepository.DeleteAllAsync(input.FilterText, input.Image, input.OrderMin, input.OrderMax, input.isActive, input.SitePropertyId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PropertyMediaDownloadTokenCacheItem { Token = token },
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