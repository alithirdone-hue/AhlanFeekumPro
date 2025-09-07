using AhlanFeekumPro.Shared;
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.Governorates;
using AhlanFeekumPro.PropertyTypes;
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
using AhlanFeekumPro.SiteProperties;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.SiteProperties
{

    [Authorize(AhlanFeekumProPermissions.SiteProperties.Default)]
    public abstract class SitePropertiesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<SitePropertyDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ISitePropertyRepository _sitePropertyRepository;
        protected SitePropertyManager _sitePropertyManager;

        protected IRepository<AhlanFeekumPro.PropertyTypes.PropertyType, Guid> _propertyTypeRepository;
        protected IRepository<AhlanFeekumPro.Governorates.Governorate, Guid> _governorateRepository;
        protected IRepository<AhlanFeekumPro.PropertyFeatures.PropertyFeature, Guid> _propertyFeatureRepository;

        public SitePropertiesAppServiceBase(ISitePropertyRepository sitePropertyRepository, SitePropertyManager sitePropertyManager, IDistributedCache<SitePropertyDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.PropertyTypes.PropertyType, Guid> propertyTypeRepository, IRepository<AhlanFeekumPro.Governorates.Governorate, Guid> governorateRepository, IRepository<AhlanFeekumPro.PropertyFeatures.PropertyFeature, Guid> propertyFeatureRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _sitePropertyRepository = sitePropertyRepository;
            _sitePropertyManager = sitePropertyManager; _propertyTypeRepository = propertyTypeRepository;
            _governorateRepository = governorateRepository;
            _propertyFeatureRepository = propertyFeatureRepository;

        }

        public virtual async Task<PagedResultDto<SitePropertyWithNavigationPropertiesDto>> GetListAsync(GetSitePropertiesInput input)
        {
            var totalCount = await _sitePropertyRepository.GetCountAsync(input.FilterText, input.PropertyTitle, input.HotelName, input.BedroomsMin, input.BedroomsMax, input.BathroomsMin, input.BathroomsMax, input.NumberOfBedMin, input.NumberOfBedMax, input.FloorMin, input.FloorMax, input.MaximumNumberOfGuestMin, input.MaximumNumberOfGuestMax, input.LivingroomsMin, input.LivingroomsMax, input.PropertyDescription, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark, input.PricePerNightMin, input.PricePerNightMax, input.IsActive, input.PropertyTypeId, input.GovernorateId, input.PropertyFeatureId);
            var items = await _sitePropertyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.PropertyTitle, input.HotelName, input.BedroomsMin, input.BedroomsMax, input.BathroomsMin, input.BathroomsMax, input.NumberOfBedMin, input.NumberOfBedMax, input.FloorMin, input.FloorMax, input.MaximumNumberOfGuestMin, input.MaximumNumberOfGuestMax, input.LivingroomsMin, input.LivingroomsMax, input.PropertyDescription, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark, input.PricePerNightMin, input.PricePerNightMax, input.IsActive, input.PropertyTypeId, input.GovernorateId, input.PropertyFeatureId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SitePropertyWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SitePropertyWithNavigationProperties>, List<SitePropertyWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<SitePropertyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<SitePropertyWithNavigationProperties, SitePropertyWithNavigationPropertiesDto>
                (await _sitePropertyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<SitePropertyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SiteProperty, SitePropertyDto>(await _sitePropertyRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPropertyTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _propertyTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AhlanFeekumPro.PropertyTypes.PropertyType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AhlanFeekumPro.PropertyTypes.PropertyType>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetGovernorateLookupAsync(LookupRequestDto input)
        {
            var query = (await _governorateRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AhlanFeekumPro.Governorates.Governorate>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AhlanFeekumPro.Governorates.Governorate>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPropertyFeatureLookupAsync(LookupRequestDto input)
        {
            var query = (await _propertyFeatureRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AhlanFeekumPro.PropertyFeatures.PropertyFeature>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AhlanFeekumPro.PropertyFeatures.PropertyFeature>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(AhlanFeekumProPermissions.SiteProperties.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _sitePropertyRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.SiteProperties.Create)]
        public virtual async Task<SitePropertyDto> CreateAsync(SitePropertyCreateDto input)
        {
            if (input.PropertyTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["PropertyType"]]);
            }
            if (input.GovernorateId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Governorate"]]);
            }

            var siteProperty = await _sitePropertyManager.CreateAsync(
            input.PropertyFeatureIds, input.PropertyTypeId, input.GovernorateId, input.PropertyTitle, input.Bedrooms, input.Bathrooms, input.NumberOfBed, input.Floor, input.MaximumNumberOfGuest, input.Livingrooms, input.PropertyDescription, input.PricePerNight, input.IsActive, input.HotelName, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark
            );

            return ObjectMapper.Map<SiteProperty, SitePropertyDto>(siteProperty);
        }

        [Authorize(AhlanFeekumProPermissions.SiteProperties.Edit)]
        public virtual async Task<SitePropertyDto> UpdateAsync(Guid id, SitePropertyUpdateDto input)
        {
            if (input.PropertyTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["PropertyType"]]);
            }
            if (input.GovernorateId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Governorate"]]);
            }

            var siteProperty = await _sitePropertyManager.UpdateAsync(
            id,
            input.PropertyFeatureIds, input.PropertyTypeId, input.GovernorateId, input.PropertyTitle, input.Bedrooms, input.Bathrooms, input.NumberOfBed, input.Floor, input.MaximumNumberOfGuest, input.Livingrooms, input.PropertyDescription, input.PricePerNight, input.IsActive, input.HotelName, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SiteProperty, SitePropertyDto>(siteProperty);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SitePropertyExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var siteProperties = await _sitePropertyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.PropertyTitle, input.HotelName, input.BedroomsMin, input.BedroomsMax, input.BathroomsMin, input.BathroomsMax, input.NumberOfBedMin, input.NumberOfBedMax, input.FloorMin, input.FloorMax, input.MaximumNumberOfGuestMin, input.MaximumNumberOfGuestMax, input.LivingroomsMin, input.LivingroomsMax, input.PropertyDescription, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark, input.PricePerNightMin, input.PricePerNightMax, input.IsActive, input.PropertyTypeId, input.GovernorateId, input.PropertyFeatureId);
            var items = siteProperties.Select(item => new
            {
                PropertyTitle = item.SiteProperty.PropertyTitle,
                HotelName = item.SiteProperty.HotelName,
                Bedrooms = item.SiteProperty.Bedrooms,
                Bathrooms = item.SiteProperty.Bathrooms,
                NumberOfBed = item.SiteProperty.NumberOfBed,
                Floor = item.SiteProperty.Floor,
                MaximumNumberOfGuest = item.SiteProperty.MaximumNumberOfGuest,
                Livingrooms = item.SiteProperty.Livingrooms,
                PropertyDescription = item.SiteProperty.PropertyDescription,
                HourseRules = item.SiteProperty.HourseRules,
                ImportantInformation = item.SiteProperty.ImportantInformation,
                Address = item.SiteProperty.Address,
                StreetAndBuildingNumber = item.SiteProperty.StreetAndBuildingNumber,
                LandMark = item.SiteProperty.LandMark,
                PricePerNight = item.SiteProperty.PricePerNight,
                IsActive = item.SiteProperty.IsActive,

                PropertyType = item.PropertyType?.Title,
                Governorate = item.Governorate?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SiteProperties.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.SiteProperties.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> sitepropertyIds)
        {
            await _sitePropertyRepository.DeleteManyAsync(sitepropertyIds);
        }

        [Authorize(AhlanFeekumProPermissions.SiteProperties.Delete)]
        public virtual async Task DeleteAllAsync(GetSitePropertiesInput input)
        {
            await _sitePropertyRepository.DeleteAllAsync(input.FilterText, input.PropertyTitle, input.HotelName, input.BedroomsMin, input.BedroomsMax, input.BathroomsMin, input.BathroomsMax, input.NumberOfBedMin, input.NumberOfBedMax, input.FloorMin, input.FloorMax, input.MaximumNumberOfGuestMin, input.MaximumNumberOfGuestMax, input.LivingroomsMin, input.LivingroomsMax, input.PropertyDescription, input.HourseRules, input.ImportantInformation, input.Address, input.StreetAndBuildingNumber, input.LandMark, input.PricePerNightMin, input.PricePerNightMax, input.IsActive, input.PropertyTypeId, input.GovernorateId, input.PropertyFeatureId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new SitePropertyDownloadTokenCacheItem { Token = token },
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