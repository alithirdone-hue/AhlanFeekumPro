using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.SiteProperties;

using System;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationWithNavigationPropertiesBase
    {
        public PropertyEvaluation PropertyEvaluation { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public SiteProperty SiteProperty { get; set; } = null!;
        

        
    }
}