using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public DateOnly? DateMin { get; set; }
        public DateOnly? DateMax { get; set; }
        public bool? IsAvailable { get; set; }
        public float? PriceMin { get; set; }
        public float? PriceMax { get; set; }
        public string? Note { get; set; }
        public Guid? SitePropertyId { get; set; }

        public PropertyCalendarExcelDownloadDtoBase()
        {

        }
    }
}