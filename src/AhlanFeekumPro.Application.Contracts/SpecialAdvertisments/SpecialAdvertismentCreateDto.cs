using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class SpecialAdvertismentCreateDtoBase
    {
        public Guid ImageId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid SitePropertyId { get; set; }
    }
}