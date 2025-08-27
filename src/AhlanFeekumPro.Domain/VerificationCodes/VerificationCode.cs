using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string PhoneOrEmail { get; set; }

        public virtual int SecurityCode { get; set; }

        public virtual bool IsExpired { get; set; }

        protected VerificationCodeBase()
        {

        }

        public VerificationCodeBase(Guid id, string phoneOrEmail, int securityCode, bool isExpired)
        {

            Id = id;
            Check.NotNull(phoneOrEmail, nameof(phoneOrEmail));
            PhoneOrEmail = phoneOrEmail;
            SecurityCode = securityCode;
            IsExpired = isExpired;
        }

    }
}