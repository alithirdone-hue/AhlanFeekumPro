using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class GetPropertyMediasInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Image { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? isActive { get; set; }
        public Guid? SitePropertyId { get; set; }

        public GetPropertyMediasInputBase()
        {

        }
    }
}