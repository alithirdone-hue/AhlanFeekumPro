using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class SpecialAdvertismentCreateDtoBase
    {
        [Required]
        public string Image { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid SitePropertyId { get; set; }
    }
}