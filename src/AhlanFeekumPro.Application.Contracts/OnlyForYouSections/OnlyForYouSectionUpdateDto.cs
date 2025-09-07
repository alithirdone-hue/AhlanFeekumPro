using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string FirstPhoto { get; set; } = null!;
        [Required]
        public string SecondPhoto { get; set; } = null!;
        [Required]
        public string ThirdPhoto { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;
    }
}