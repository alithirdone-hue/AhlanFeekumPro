namespace AhlanFeekumPro.PropertyMedias
{
    public static class PropertyMediaConsts
    {
        private const string DefaultSorting = "{0}Image asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PropertyMedia." : string.Empty);
        }

    }
}