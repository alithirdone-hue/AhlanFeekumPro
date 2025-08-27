using System;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeExcelDtoBase
    {
        public string PhoneOrEmail { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsExpired { get; set; }
    }
}