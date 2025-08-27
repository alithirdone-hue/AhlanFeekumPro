using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class UserProfileExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool? IsSuperHost { get; set; }
        public Guid? IdentityRoleId { get; set; }
        public Guid? IdentityUserId { get; set; }

        public UserProfileExcelDownloadDtoBase()
        {

        }
    }
}