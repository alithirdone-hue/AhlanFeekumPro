using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.PropertyFeatures;

using System;
using System.Collections.Generic;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyWithNavigationPropertiesBase
    {
        public SiteProperty SiteProperty { get; set; } = null!;

        public PropertyType PropertyType { get; set; } = null!;
        

        public List<PropertyFeature> PropertyFeatures { get; set; } = null!;
        
    }
}