using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string FirstPhoto { get; set; } = null!;
        public string SecondPhoto { get; set; } = null!;
        public string ThirdPhoto { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}