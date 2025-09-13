namespace AhlanFeekumPro.PropertyCalendars
{
    public static class PropertyCalendarConsts
    {
        private const string DefaultSorting = "{0}Date asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PropertyCalendar." : string.Empty);
        }

    }
}