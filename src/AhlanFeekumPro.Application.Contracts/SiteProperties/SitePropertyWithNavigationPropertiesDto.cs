using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.Governorates;
using AhlanFeekumPro.PropertyFeatures;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyWithNavigationPropertiesDtoBase
    {
        public SitePropertyDto SiteProperty { get; set; } = null!;

        public PropertyTypeDto PropertyType { get; set; } = null!;
        public GovernorateDto Governorate { get; set; } = null!;
        public List<PropertyFeatureDto> PropertyFeatures { get; set; } = new List<PropertyFeatureDto>();

    }
}