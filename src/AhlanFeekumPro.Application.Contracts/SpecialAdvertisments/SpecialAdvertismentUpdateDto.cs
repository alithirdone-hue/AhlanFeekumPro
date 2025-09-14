using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class SpecialAdvertismentUpdateDtoBase : IHasConcurrencyStamp
    {
        public Guid ImageId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}