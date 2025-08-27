using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyTypes
{
    public abstract class PropertyTypeDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}