using System;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarExcelDtoBase
    {
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; }
        public float? Price { get; set; }
        public string? Note { get; set; }
    }
}