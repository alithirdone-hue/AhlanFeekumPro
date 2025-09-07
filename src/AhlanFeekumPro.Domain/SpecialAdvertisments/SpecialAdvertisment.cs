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

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class SpecialAdvertismentBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Image { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid SitePropertyId { get; set; }

        protected SpecialAdvertismentBase()
        {

        }

        public SpecialAdvertismentBase(Guid id, Guid sitePropertyId, string image, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(image, nameof(image));
            Image = image;
            Order = order;
            IsActive = isActive;
            SitePropertyId = sitePropertyId;
        }

    }
}