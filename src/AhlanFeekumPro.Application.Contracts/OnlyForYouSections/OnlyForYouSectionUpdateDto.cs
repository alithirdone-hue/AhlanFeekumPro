using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionUpdateDtoBase : IHasConcurrencyStamp
    {
        public Guid FirstPhotoId { get; set; }
        public Guid SecondPhotoId { get; set; }
        public Guid ThirdPhotoId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}