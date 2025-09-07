using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.Governorates
{
    public abstract class GovernorateBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        protected GovernorateBase()
        {

        }

        public GovernorateBase(Guid id, string title, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Title = title;
            Order = order;
            IsActive = isActive;
        }

    }
}