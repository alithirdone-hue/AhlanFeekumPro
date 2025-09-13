using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarUpdateDtoBase : IHasConcurrencyStamp
    {
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; }
        public float? Price { get; set; }
        public string? Note { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}