using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class PropertyMediaUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Image { get; set; } = null!;
        public int Order { get; set; }
        public bool isActive { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}