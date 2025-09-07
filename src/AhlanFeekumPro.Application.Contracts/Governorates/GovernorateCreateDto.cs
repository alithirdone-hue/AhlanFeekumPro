using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.Governorates
{
    public abstract class GovernorateCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}