using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.SiteProperties;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationWithNavigationPropertiesDtoBase
    {
        public PropertyEvaluationDto PropertyEvaluation { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;
        public SitePropertyDto SiteProperty { get; set; } = null!;

    }
}