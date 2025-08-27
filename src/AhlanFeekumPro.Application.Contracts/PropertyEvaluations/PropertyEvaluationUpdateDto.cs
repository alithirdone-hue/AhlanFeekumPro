using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        [Range(PropertyEvaluationConsts.CleanlinessMinLength, PropertyEvaluationConsts.CleanlinessMaxLength)]
        public int Cleanliness { get; set; }
        [Required]
        [Range(PropertyEvaluationConsts.PriceAndValueMinLength, PropertyEvaluationConsts.PriceAndValueMaxLength)]
        public int PriceAndValue { get; set; }
        [Required]
        [Range(PropertyEvaluationConsts.LocationMinLength, PropertyEvaluationConsts.LocationMaxLength)]
        public int Location { get; set; }
        [Required]
        [Range(PropertyEvaluationConsts.AccuracyMinLength, PropertyEvaluationConsts.AccuracyMaxLength)]
        public int Accuracy { get; set; }
        [Required]
        [Range(PropertyEvaluationConsts.AttitudeMinLength, PropertyEvaluationConsts.AttitudeMaxLength)]
        public int Attitude { get; set; }
        public string? RatingComment { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid SitePropertyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}