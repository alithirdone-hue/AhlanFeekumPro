using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; }
        public float? Price { get; set; }
        public string? Note { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}