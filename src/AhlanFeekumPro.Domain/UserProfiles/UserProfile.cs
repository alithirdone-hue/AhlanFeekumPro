using Volo.Abp.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Email { get; set; }

        [CanBeNull]
        public virtual string? PhoneNumber { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }

        [CanBeNull]
        public virtual string? Address { get; set; }

        [CanBeNull]
        public virtual string? ProfilePhoto { get; set; }

        public virtual bool IsSuperHost { get; set; }
        public Guid? IdentityRoleId { get; set; }
        public Guid IdentityUserId { get; set; }

        protected UserProfileBase()
        {

        }

        public UserProfileBase(Guid id, Guid? identityRoleId, Guid identityUserId, string name, bool isSuperHost, string? email = null, string? phoneNumber = null, string? latitude = null, string? longitude = null, string? address = null, string? profilePhoto = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            IsSuperHost = isSuperHost;
            Email = email;
            PhoneNumber = phoneNumber;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            ProfilePhoto = profilePhoto;
            IdentityRoleId = identityRoleId;
            IdentityUserId = identityUserId;
        }

    }
}