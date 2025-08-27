using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string PhoneOrEmail { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsExpired { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}