namespace AhlanFeekumPro.FavoriteProperties
{
    public static class FavoritePropertyConsts
    {
        private const string DefaultSorting = "{0}Id asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "FavoriteProperty." : string.Empty);
        }

    }
}