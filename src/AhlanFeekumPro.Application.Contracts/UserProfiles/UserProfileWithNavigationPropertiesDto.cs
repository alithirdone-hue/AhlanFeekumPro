using Volo.Abp.Identity;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileWithNavigationPropertiesDtoBase
    {
        public UserProfileDto UserProfile { get; set; } = null!;

        public IdentityRoleDto IdentityRole { get; set; } = null!;
        public IdentityUserDto IdentityUser { get; set; } = null!;

    }
}