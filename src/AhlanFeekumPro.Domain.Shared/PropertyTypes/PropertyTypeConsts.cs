namespace AhlanFeekumPro.PropertyTypes
{
    public static class PropertyTypeConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PropertyType." : string.Empty);
        }

    }
}