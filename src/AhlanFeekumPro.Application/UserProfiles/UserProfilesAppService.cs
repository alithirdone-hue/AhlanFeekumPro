using AhlanFeekumPro.Shared;
using Volo.Abp.Identity;
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
using AhlanFeekumPro.UserProfiles;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.UserProfiles
{

    [Authorize(AhlanFeekumProPermissions.UserProfiles.Default)]
    public abstract class UserProfilesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<UserProfileDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserProfileRepository _userProfileRepository;
        protected UserProfileManager _userProfileManager;

        protected IRepository<Volo.Abp.Identity.IdentityRole, Guid> _identityRoleRepository;
        protected IRepository<Volo.Abp.Identity.IdentityUser, Guid> _identityUserRepository;

        public UserProfilesAppServiceBase(IUserProfileRepository userProfileRepository, UserProfileManager userProfileManager, IDistributedCache<UserProfileDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Volo.Abp.Identity.IdentityRole, Guid> identityRoleRepository, IRepository<Volo.Abp.Identity.IdentityUser, Guid> identityUserRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _userProfileRepository = userProfileRepository;
            _userProfileManager = userProfileManager; _identityRoleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;

        }

        public virtual async Task<PagedResultDto<UserProfileWithNavigationPropertiesDto>> GetListAsync(GetUserProfilesInput input)
        {
            var totalCount = await _userProfileRepository.GetCountAsync(input.FilterText, input.Name, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto, input.IsSuperHost, input.IdentityRoleId, input.IdentityUserId);
            var items = await _userProfileRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto, input.IsSuperHost, input.IdentityRoleId, input.IdentityUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserProfileWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserProfileWithNavigationProperties>, List<UserProfileWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserProfileWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserProfileWithNavigationProperties, UserProfileWithNavigationPropertiesDto>
                (await _userProfileRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserProfileDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserProfile, UserProfileDto>(await _userProfileRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIdentityRoleLookupAsync(LookupRequestDto input)
        {
            var query = (await _identityRoleRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Volo.Abp.Identity.IdentityRole>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Volo.Abp.Identity.IdentityRole>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _identityUserRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.UserName != null &&
                         x.UserName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Volo.Abp.Identity.IdentityUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Volo.Abp.Identity.IdentityUser>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(AhlanFeekumProPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userProfileRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.UserProfiles.Create)]
        public virtual async Task<UserProfileDto> CreateAsync(UserProfileCreateDto input)
        {
            if (input.IdentityUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["IdentityUser"]]);
            }

            var userProfile = await _userProfileManager.CreateAsync(
            input.IdentityRoleId, input.IdentityUserId, input.Name, input.IsSuperHost, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto
            );

            return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
        }

        [Authorize(AhlanFeekumProPermissions.UserProfiles.Edit)]
        public virtual async Task<UserProfileDto> UpdateAsync(Guid id, UserProfileUpdateDto input)
        {
            if (input.IdentityUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["IdentityUser"]]);
            }

            var userProfile = await _userProfileManager.UpdateAsync(
            id,
            input.IdentityRoleId, input.IdentityUserId, input.Name, input.IsSuperHost, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserProfileExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var userProfiles = await _userProfileRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto, input.IsSuperHost, input.IdentityRoleId, input.IdentityUserId);
            var items = userProfiles.Select(item => new
            {
                Name = item.UserProfile.Name,
                Email = item.UserProfile.Email,
                PhoneNumber = item.UserProfile.PhoneNumber,
                Latitude = item.UserProfile.Latitude,
                Longitude = item.UserProfile.Longitude,
                Address = item.UserProfile.Address,
                ProfilePhoto = item.UserProfile.ProfilePhoto,
                IsSuperHost = item.UserProfile.IsSuperHost,

                IdentityRole = item.IdentityRole?.Name,
                IdentityUser = item.IdentityUser?.UserName,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserProfiles.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> userprofileIds)
        {
            await _userProfileRepository.DeleteManyAsync(userprofileIds);
        }

        [Authorize(AhlanFeekumProPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteAllAsync(GetUserProfilesInput input)
        {
            await _userProfileRepository.DeleteAllAsync(input.FilterText, input.Name, input.Email, input.PhoneNumber, input.Latitude, input.Longitude, input.Address, input.ProfilePhoto, input.IsSuperHost, input.IdentityRoleId, input.IdentityUserId);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserProfileDownloadTokenCacheItem { Token = token },
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