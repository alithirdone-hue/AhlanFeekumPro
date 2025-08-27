using AhlanFeekumPro.SiteProperties;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class PropertyMediaBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Image { get; set; }

        public virtual int Order { get; set; }

        public virtual bool isActive { get; set; }
        public Guid SitePropertyId { get; set; }

        protected PropertyMediaBase()
        {

        }

        public PropertyMediaBase(Guid id, Guid sitePropertyId, string image, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(image, nameof(image));
            Image = image;
            Order = order;
            this.isActive = isActive;
            SitePropertyId = sitePropertyId;
        }

    }
}