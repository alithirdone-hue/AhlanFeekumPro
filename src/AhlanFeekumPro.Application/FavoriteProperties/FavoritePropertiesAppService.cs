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
using AhlanFeekumPro.FavoriteProperties;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.FavoriteProperties
{

    [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Default)]
    public abstract class FavoritePropertiesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<FavoritePropertyDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IFavoritePropertyRepository _favoritePropertyRepository;
        protected FavoritePropertyManager _favoritePropertyManager;

        protected IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> _sitePropertyRepository;

        public FavoritePropertiesAppServiceBase(IFavoritePropertyRepository favoritePropertyRepository, FavoritePropertyManager favoritePropertyManager, IDistributedCache<FavoritePropertyDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AhlanFeekumPro.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<AhlanFeekumPro.SiteProperties.SiteProperty, Guid> sitePropertyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _favoritePropertyRepository = favoritePropertyRepository;
            _favoritePropertyManager = favoritePropertyManager; _userProfileRepository = userProfileRepository;
            _sitePropertyRepository = sitePropertyRepository;

        }

        public virtual async Task<PagedResultDto<FavoritePropertyWithNavigationPropertiesDto>> GetListAsync(GetFavoritePropertiesInput input)
        {
            var totalCount = await _favoritePropertyRepository.GetCountAsync(input.FilterText, input.UserProfileId, input.SitePropertyId);
            var items = await _favoritePropertyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.UserProfileId, input.SitePropertyId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<FavoritePropertyWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<FavoritePropertyWithNavigationProperties>, List<FavoritePropertyWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<FavoritePropertyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<FavoritePropertyWithNavigationProperties, FavoritePropertyWithNavigationPropertiesDto>
                (await _favoritePropertyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<FavoritePropertyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<FavoriteProperty, FavoritePropertyDto>(await _favoritePropertyRepository.GetAsync(id));
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

        [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _favoritePropertyRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Create)]
        public virtual async Task<FavoritePropertyDto> CreateAsync(FavoritePropertyCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var favoriteProperty = await _favoritePropertyManager.CreateAsync(
            input.UserProfileId, input.SitePropertyId
            );

            return ObjectMapper.Map<FavoriteProperty, FavoritePropertyDto>(favoriteProperty);
        }

        [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Edit)]
        public virtual async Task<FavoritePropertyDto> UpdateAsync(Guid id, FavoritePropertyUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.SitePropertyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SiteProperty"]]);
            }

            var favoriteProperty = await _favoritePropertyManager.UpdateAsync(
            id,
            input.UserProfileId, input.SitePropertyId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<FavoriteProperty, FavoritePropertyDto>(favoriteProperty);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(FavoritePropertyExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var favoriteProperties = await _favoritePropertyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.UserProfileId, input.SitePropertyId);
            var items = favoriteProperties.Select(item => new
            {

                UserProfile = item.UserProfile?.Name,
                SiteProperty = item.SiteProperty?.PropertyTitle,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "FavoriteProperties.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> favoritepropertyIds)
        {
            await _favoritePropertyRepository.DeleteManyAsync(favoritepropertyIds);
        }

        [Authorize(AhlanFeekumProPermissions.FavoriteProperties.Delete)]
        public virtual async Task DeleteAllAsync(GetFavoritePropertiesInput input)
        {
            await _favoritePropertyRepository.DeleteAllAsync(input.FilterText, input.UserProfileId, input.SitePropertyId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new FavoritePropertyDownloadTokenCacheItem { Token = token },
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