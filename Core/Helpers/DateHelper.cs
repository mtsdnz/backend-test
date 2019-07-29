using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Helpers
{
    public static class DateHelper
    {
        public static DateTime TryParseWithCultures(string s, string format, string[] cultures)
        {
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(s, format, CultureInfo.GetCultureInfo(culture),
                    DateTimeStyles.None,
                    out var date))
                {
                    return date;
                }
            }

            return DateTime.Parse(s, CultureInfo.CurrentCulture);
        }
    }
}