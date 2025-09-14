namespace AhlanFeekumPro.SpecialAdvertisments
{
    public static class SpecialAdvertismentConsts
    {
        private const string DefaultSorting = "{0}ImageId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SpecialAdvertisment." : string.Empty);
        }

    }
}