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
using AhlanFeekumPro.PropertyCalendars;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyCalendars
{

    [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Default)]
    public abstract class PropertyCalendarsAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<PropertyCalendarDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPropertyCalendarRepository _propertyCalendarRepository;
        protected PropertyCalendarManager _propertyCalendarManager;

        protected IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> _sitePropertyRepository;

        public PropertyCalendarsAppServiceBase(IPropertyCalendarRepository propertyCalendarRepository, PropertyCalendarManager propertyCalendarManager, IDistributedCache<PropertyCalendarDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> sitePropertyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _propertyCalendarRepository = propertyCalendarRepository;
            _propertyCalendarManager = propertyCalendarManager; _sitePropertyRepository = sitePropertyRepository;

        }

        public virtual async Task<PagedResultDto<PropertyCalendarWithNavigationPropertiesDto>> GetListAsync(GetPropertyCalendarsInput input)
        {
            var totalCount = await _propertyCalendarRepository.GetCountAsync(input.FilterText, input.DateMin, input.DateMax, input.IsAvailable, input.PriceMin, input.PriceMax, input.Note, input.SitePropertyId);
            var items = await _propertyCalendarRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.DateMin, input.DateMax, input.IsAvailable, input.PriceMin, input.PriceMax, input.Note, input.SitePropertyId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PropertyCalendarWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PropertyCalendarWithNavigationProperties>, List<PropertyCalendarWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PropertyCalendarWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyCalendarWithNavigationProperties, PropertyCalendarWithNavigationPropertiesDto>
                (await _propertyCalendarRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<PropertyCalendarDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PropertyCalendar, PropertyCalendarDto>(await _propertyCalendarRepository.GetAsync(id));
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

        [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _propertyCalendarRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Create)]
        public virtual async Task<PropertyCalendarDto> CreateAsync(PropertyCalendarCreateDto input)
        {
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyCalendar = await _propertyCalendarManager.CreateAsync(
            input.SitePropertyId, input.Date, input.IsAvailable, input.Price, input.Note
            );

            return ObjectMapper.Map<PropertyCalendar, PropertyCalendarDto>(propertyCalendar);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Edit)]
        public virtual async Task<PropertyCalendarDto> UpdateAsync(Guid id, PropertyCalendarUpdateDto input)
        {
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var propertyCalendar = await _propertyCalendarManager.UpdateAsync(
            id,
            input.SitePropertyId, input.Date, input.IsAvailable, input.Price, input.Note, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PropertyCalendar, PropertyCalendarDto>(propertyCalendar);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyCalendarExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var propertyCalendars = await _propertyCalendarRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.DateMin, input.DateMax, input.IsAvailable, input.PriceMin, input.PriceMax, input.Note, input.SitePropertyId);
            var items = propertyCalendars.Select(item => new
            {
                Date = item.PropertyCalendar.Date,
                IsAvailable = item.PropertyCalendar.IsAvailable,
                Price = item.PropertyCalendar.Price,
                Note = item.PropertyCalendar.Note,

                SiteProperty = item.SiteProperty?.PropertyTitle,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PropertyCalendars.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> propertycalendarIds)
        {
            await _propertyCalendarRepository.DeleteManyAsync(propertycalendarIds);
        }

        [Authorize(AhlanFeekumProPermissions.PropertyCalendars.Delete)]
        public virtual async Task DeleteAllAsync(GetPropertyCalendarsInput input)
        {
            await _propertyCalendarRepository.DeleteAllAsync(input.FilterText, input.DateMin, input.DateMax, input.IsAvailable, input.PriceMin, input.PriceMax, input.Note, input.SitePropertyId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PropertyCalendarDownloadTokenCacheItem { Token = token },
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