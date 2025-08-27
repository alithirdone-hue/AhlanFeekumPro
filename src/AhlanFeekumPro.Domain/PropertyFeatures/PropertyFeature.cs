using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Icon { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        protected PropertyFeatureBase()
        {

        }

        public PropertyFeatureBase(Guid id, string title, string icon, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Check.NotNull(icon, nameof(icon));
            Title = title;
            Icon = icon;
            Order = order;
            IsActive = isActive;
        }

    }
}