using System;

namespace Sortiously.Framework
{
    public static class SortHelpers
    {

        public static long JulianDateForSort(DateTime date)
        {
            int month = date.Month;
            int day = date.Day;
            int year = date.Year;

            if (month < 3)
            {
                month = month + 12;
                year = year - 1;
            }
            //modified Julian Date
            return (long)(day + (153 * month - 457) / 5 + 365 * year + (year / 4) - (year / 100) + (year / 400) - 678882);
        }

        internal static string GetParameterName<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            return item.ToString().TrimStart('{').TrimEnd('}').Split('=')[0].Trim();
        }

    }
}
