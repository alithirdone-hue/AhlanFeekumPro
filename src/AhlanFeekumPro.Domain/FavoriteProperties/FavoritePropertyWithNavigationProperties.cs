using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.SiteProperties;

using System;
using System.Collections.Generic;

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class FavoritePropertyWithNavigationPropertiesBase
    {
        public FavoriteProperty FavoriteProperty { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public SiteProperty SiteProperty { get; set; } = null!;
        

        
    }
}