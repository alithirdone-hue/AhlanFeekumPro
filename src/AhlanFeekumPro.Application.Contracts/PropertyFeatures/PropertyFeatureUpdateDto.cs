using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Icon { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}