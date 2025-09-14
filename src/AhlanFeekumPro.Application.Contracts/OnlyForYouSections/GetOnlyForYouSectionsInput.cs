using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class GetOnlyForYouSectionsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public GetOnlyForYouSectionsInputBase()
        {

        }
    }
}