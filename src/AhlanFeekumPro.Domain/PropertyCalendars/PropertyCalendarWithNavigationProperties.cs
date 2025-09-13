using AhlanFeekumPro.SiteProperties;

using System;
using System.Collections.Generic;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarWithNavigationPropertiesBase
    {
        public PropertyCalendar PropertyCalendar { get; set; } = null!;

        public SiteProperty SiteProperty { get; set; } = null!;
        

        
    }
}