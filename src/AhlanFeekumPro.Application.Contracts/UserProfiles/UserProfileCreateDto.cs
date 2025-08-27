using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileCreateDtoBase
    {
        [Required]
        public string Name { get; set; } = null!;
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsSuperHost { get; set; } = false;
        public Guid? IdentityRoleId { get; set; }
        public Guid IdentityUserId { get; set; }
    }
}