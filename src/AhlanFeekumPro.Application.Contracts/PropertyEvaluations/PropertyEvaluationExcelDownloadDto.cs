using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public int? CleanlinessMin { get; set; }
        public int? CleanlinessMax { get; set; }
        public int? PriceAndValueMin { get; set; }
        public int? PriceAndValueMax { get; set; }
        public int? LocationMin { get; set; }
        public int? LocationMax { get; set; }
        public int? AccuracyMin { get; set; }
        public int? AccuracyMax { get; set; }
        public int? AttitudeMin { get; set; }
        public int? AttitudeMax { get; set; }
        public string? RatingComment { get; set; }
        public Guid? UserProfileId { get; set; }
        public Guid? SitePropertyId { get; set; }

        public PropertyEvaluationExcelDownloadDtoBase()
        {

        }
    }
}