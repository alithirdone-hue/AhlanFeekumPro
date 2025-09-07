using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? FirstPhoto { get; set; }
        public string? SecondPhoto { get; set; }
        public string? ThirdPhoto { get; set; }

        public OnlyForYouSectionExcelDownloadDtoBase()
        {

        }
    }
}