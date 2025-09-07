namespace AhlanFeekumPro.OnlyForYouSections
{
    public static class OnlyForYouSectionConsts
    {
        private const string DefaultSorting = "{0}FirstPhoto asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "OnlyForYouSection." : string.Empty);
        }

    }
}