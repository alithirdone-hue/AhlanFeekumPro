namespace AhlanFeekumPro.Governorates
{
    public static class GovernorateConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Governorate." : string.Empty);
        }

    }
}