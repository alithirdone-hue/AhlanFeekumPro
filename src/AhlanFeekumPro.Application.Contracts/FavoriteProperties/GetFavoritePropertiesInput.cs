using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class GetFavoritePropertiesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public Guid? UserProfileId { get; set; }
        public Guid? SitePropertyId { get; set; }

        public GetFavoritePropertiesInputBase()
        {

        }
    }
}