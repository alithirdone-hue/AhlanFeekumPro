using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int Cleanliness { get; set; }
        public int PriceAndValue { get; set; }
        public int Location { get; set; }
        public int Accuracy { get; set; }
        public int Attitude { get; set; }
        public string? RatingComment { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}