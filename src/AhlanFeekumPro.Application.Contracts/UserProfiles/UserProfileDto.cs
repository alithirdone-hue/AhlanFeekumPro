using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsSuperHost { get; set; }
        public Guid? IdentityRoleId { get; set; }
        public Guid IdentityUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}