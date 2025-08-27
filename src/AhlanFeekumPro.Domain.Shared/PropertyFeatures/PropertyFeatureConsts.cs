namespace AhlanFeekumPro.PropertyFeatures
{
    public static class PropertyFeatureConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PropertyFeature." : string.Empty);
        }

    }
}