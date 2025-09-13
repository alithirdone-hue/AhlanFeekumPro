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

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual DateOnly Date { get; set; }

        public virtual bool IsAvailable { get; set; }

        public virtual float? Price { get; set; }

        [CanBeNull]
        public virtual string? Note { get; set; }
        public Guid SitePropertyId { get; set; }

        protected PropertyCalendarBase()
        {

        }

        public PropertyCalendarBase(Guid id, Guid sitePropertyId, DateOnly date, bool isAvailable, float? price = null, string? note = null)
        {

            Id = id;
            Date = date;
            IsAvailable = isAvailable;
            Price = price;
            Note = note;
            SitePropertyId = sitePropertyId;
        }

    }
}