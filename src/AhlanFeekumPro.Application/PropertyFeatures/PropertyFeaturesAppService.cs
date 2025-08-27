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
using AhlanFeekumPro.PropertyFeatures;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyFeatures
{

    [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Default)]
    public abstract class PropertyFeaturesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<PropertyFeatureDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPropertyFeatureRepository _propertyFeatureRepository;
        protected PropertyFeatureManager _propertyFeatureManager;

        public PropertyFeaturesAppServiceBase(IPropertyFeatureRepository propertyFeatureRepository, PropertyFeatureManager propertyFeatureManager, IDistributedCache<PropertyFeatureDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _propertyFeatureRepository = propertyFeatureRepository;
            _propertyFeatureManager = propertyFeatureManager;

        }

        public virtual async Task<PagedResultDto<PropertyFeatureDto>> GetListAsync(GetPropertyFeaturesInput input)
        {
            var totalCount = await _propertyFeatureRepository.GetCountAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _propertyFeatureRepository.GetListAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PropertyFeatureDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PropertyFeature>, List<PropertyFeatureDto>>(items)
            };
        }

        public virtual async Task<PropertyFeatureDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyFeature, PropertyFeatureDto>(await _propertyFeatureRepository.GetAsync(id));
        }

        [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _propertyFeatureRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Create)]
        public virtual async Task<PropertyFeatureDto> CreateAsync(PropertyFeatureCreateDto input)
        {

            var propertyFeature = await _propertyFeatureManager.CreateAsync(
            input.Title, input.Icon, input.Order, input.IsActive
            );

            return ObjectMapper.Map<PropertyFeature, PropertyFeatureDto>(propertyFeature);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Edit)]
        public virtual async Task<PropertyFeatureDto> UpdateAsync(Guid id, PropertyFeatureUpdateDto input)
        {

            var propertyFeature = await _propertyFeatureManager.UpdateAsync(
            id,
            input.Title, input.Icon, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PropertyFeature, PropertyFeatureDto>(propertyFeature);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyFeatureExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _propertyFeatureRepository.GetListAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<PropertyFeature>, List<PropertyFeatureExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PropertyFeatures.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> propertyfeatureIds)
        {
            await _propertyFeatureRepository.DeleteManyAsync(propertyfeatureIds);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyFeatures.Delete)]
        public virtual async Task DeleteAllAsync(GetPropertyFeaturesInput input)
        {
            await _propertyFeatureRepository.DeleteAllAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PropertyFeatureDownloadTokenCacheItem { Token = token },
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