using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Icon { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public PropertyFeatureExcelDownloadDtoBase()
        {

        }
    }
}