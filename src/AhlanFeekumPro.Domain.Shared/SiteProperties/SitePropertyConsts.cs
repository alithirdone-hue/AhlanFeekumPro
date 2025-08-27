namespace AhlanFeekumPro.SiteProperties
{
    public static class SitePropertyConsts
    {
        private const string DefaultSorting = "{0}PropertyTitle asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SiteProperty." : string.Empty);
        }

    }
}