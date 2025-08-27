using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? PhoneOrEmail { get; set; }
        public int? SecurityCodeMin { get; set; }
        public int? SecurityCodeMax { get; set; }
        public bool? IsExpired { get; set; }

        public VerificationCodeExcelDownloadDtoBase()
        {

        }
    }
}