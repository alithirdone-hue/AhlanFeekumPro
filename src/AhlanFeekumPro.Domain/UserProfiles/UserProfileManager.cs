using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileManagerBase : DomainService
    {
        protected IUserProfileRepository _userProfileRepository;

        public UserProfileManagerBase(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public virtual async Task<UserProfile> CreateAsync(
        Guid? identityRoleId, Guid identityUserId, string name, bool isSuperHost, string? email = null, string? phoneNumber = null, string? latitude = null, string? longitude = null, string? address = null, string? profilePhoto = null)
        {
            Check.NotNull(identityUserId, nameof(identityUserId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var userProfile = new UserProfile(
             GuidGenerator.Create(),
             identityRoleId, identityUserId, name, isSuperHost, email, phoneNumber, latitude, longitude, address, profilePhoto
             );

            return await _userProfileRepository.InsertAsync(userProfile);
        }

        public virtual async Task<UserProfile> UpdateAsync(
            Guid id,
            Guid? identityRoleId, Guid identityUserId, string name, bool isSuperHost, string? email = null, string? phoneNumber = null, string? latitude = null, string? longitude = null, string? address = null, string? profilePhoto = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(identityUserId, nameof(identityUserId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var userProfile = await _userProfileRepository.GetAsync(id);

            userProfile.IdentityRoleId = identityRoleId;
            userProfile.IdentityUserId = identityUserId;
            userProfile.Name = name;
            userProfile.IsSuperHost = isSuperHost;
            userProfile.Email = email;
            userProfile.PhoneNumber = phoneNumber;
            userProfile.Latitude = latitude;
            userProfile.Longitude = longitude;
            userProfile.Address = address;
            userProfile.ProfilePhoto = profilePhoto;

            userProfile.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userProfileRepository.UpdateAsync(userProfile);
        }

    }
}