using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class FavoritePropertyUpdateDtoBase : IHasConcurrencyStamp
    {

        public Guid UserProfileId { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}