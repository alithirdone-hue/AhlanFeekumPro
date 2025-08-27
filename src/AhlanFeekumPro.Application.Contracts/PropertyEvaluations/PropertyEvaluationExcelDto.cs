using System;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationExcelDtoBase
    {
        public int Cleanliness { get; set; }
        public int PriceAndValue { get; set; }
        public int Location { get; set; }
        public int Accuracy { get; set; }
        public int Attitude { get; set; }
        public string? RatingComment { get; set; }
    }
}