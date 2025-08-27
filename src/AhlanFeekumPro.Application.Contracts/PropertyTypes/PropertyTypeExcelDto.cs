using System;

namespace AhlanFeekumPro.PropertyTypes
{
    public abstract class PropertyTypeExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}