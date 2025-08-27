using System;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}