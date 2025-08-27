using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Icon { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}