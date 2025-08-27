using AhlanFeekumPro.SiteProperties;

using System;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class PropertyMediaWithNavigationPropertiesBase
    {
        public PropertyMedia PropertyMedia { get; set; } = null!;

        public SiteProperty SiteProperty { get; set; } = null!;
        

        
    }
}