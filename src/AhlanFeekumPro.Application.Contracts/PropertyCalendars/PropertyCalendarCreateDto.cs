using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarCreateDtoBase
    {
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; } = false;
        public float? Price { get; set; }
        public string? Note { get; set; }
        public Guid SitePropertyId { get; set; }
    }
}