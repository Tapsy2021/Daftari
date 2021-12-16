using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public static class ExtensionMethods
    {
        public static IEnumerable<DateTime> To(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
    }
}
