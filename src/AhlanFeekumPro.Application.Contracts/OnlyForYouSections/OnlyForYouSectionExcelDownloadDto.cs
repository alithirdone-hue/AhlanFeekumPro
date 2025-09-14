using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public OnlyForYouSectionExcelDownloadDtoBase()
        {

        }
    }
}