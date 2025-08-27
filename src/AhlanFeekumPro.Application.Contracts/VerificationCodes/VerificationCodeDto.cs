using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string PhoneOrEmail { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsExpired { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}