using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class PropertyMediaDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Image { get; set; } = null!;
        public int Order { get; set; }
        public bool isActive { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}