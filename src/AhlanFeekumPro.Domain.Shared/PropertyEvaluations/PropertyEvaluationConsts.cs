namespace AhlanFeekumPro.PropertyEvaluations
{
    public static class PropertyEvaluationConsts
    {
        private const string DefaultSorting = "{0}Cleanliness asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PropertyEvaluation." : string.Empty);
        }

        public const int CleanlinessMinLength = 1;
        public const int CleanlinessMaxLength = 10;
        public const int PriceAndValueMinLength = 1;
        public const int PriceAndValueMaxLength = 10;
        public const int LocationMinLength = 1;
        public const int LocationMaxLength = 10;
        public const int AccuracyMinLength = 1;
        public const int AccuracyMaxLength = 10;
        public const int AttitudeMinLength = 1;
        public const int AttitudeMaxLength = 10;
    }
}