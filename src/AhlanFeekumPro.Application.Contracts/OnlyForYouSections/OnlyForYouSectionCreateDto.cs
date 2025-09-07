using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionCreateDtoBase
    {
        [Required]
        public string FirstPhoto { get; set; } = null!;
        [Required]
        public string SecondPhoto { get; set; } = null!;
        [Required]
        public string ThirdPhoto { get; set; } = null!;
    }
}