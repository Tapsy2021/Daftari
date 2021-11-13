using Daftari.Helpers;
using System;
using System.Collections.Generic;

namespace Daftari.Handlers
{
    public static class DataTypeExtensions
    {
        /// <summary>
        /// Converts the value of the client DateTime object to Server Local Time
        /// </summary>
        public static DateTime ToServerTime(this DateTime thisDate)
        {
            var TimezoneOffset = CookieHelper.GetFromCookie("Session", "TimezoneOffset");
            if (!string.IsNullOrEmpty(TimezoneOffset) && double.TryParse(TimezoneOffset, out double ClientOffset))
            {
                //var offset = ClientOffset - DateTimeOffset.Now.Offset.Minutes; 
                return thisDate.AddMinutes(ClientOffset).ToLocalTime();
            }
            return thisDate;
        }
        /// <summary>
        /// Converts the value of the server DateTime object to Client Local Time
        /// </summary>
        public static DateTime ToClientTime(this DateTime thisDate)
        {
            var TimezoneOffset = CookieHelper.GetFromCookie("Session", "TimezoneOffset");
            if (!string.IsNullOrEmpty(TimezoneOffset) && double.TryParse(TimezoneOffset, out double ClientOffset))
            {
                return DateTime.SpecifyKind(thisDate, DateTime.Now.Kind).ToUniversalTime().AddMinutes(ClientOffset * -1);
            }
            return thisDate;
        }
        public static bool IsNumeric(this string thisString)
        {
            return double.TryParse(thisString, out double isnum);
        }

        public static IEnumerable<DateTime> To(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
    }
}