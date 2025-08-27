using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class FavoritePropertyExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public Guid? UserProfileId { get; set; }
        public Guid? SitePropertyId { get; set; }

        public FavoritePropertyExcelDownloadDtoBase()
        {

        }
    }
}