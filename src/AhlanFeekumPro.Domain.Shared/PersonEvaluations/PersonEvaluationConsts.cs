namespace AhlanFeekumPro.PersonEvaluations
{
    public static class PersonEvaluationConsts
    {
        private const string DefaultSorting = "{0}Rate asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PersonEvaluation." : string.Empty);
        }

        public const int RateMinLength = 1;
        public const int RateMaxLength = 10;
    }
}