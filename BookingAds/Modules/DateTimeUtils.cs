using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using BookingAds.Constants;

namespace BookingAds.Modules
{
    public static class DateTimeUtils
    {
        public static long ConvertToUnixTimeSeconds(this DateTime date)
        {
            long unixTime = ((DateTimeOffset)date).ToUnixTimeSeconds();
            return unixTime;
        }

        public static string ConvertToExposeTime(this DateTime date, string format = "dd/MM/yyyy HH:mm:ss tt")
        {
            if (date == null)
            {
                return null;
            }

            return Convert.ToDateTime(date).ToString(format);
        }

        public static DateTime? ConvertToDateTimeSQL(string dateTimeString, string format = "yyyy-MM-ddTHH:mm")
        {
            try
            {
                return DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidDateTimeSQL(string dateTimeString)
        {
            var dateTime = ConvertToDateTimeSQL(dateTimeString);
            return string.IsNullOrEmpty(dateTimeString)
                || (!string.IsNullOrEmpty(dateTimeString)
                    && dateTime != null
                    && dateTime >= DateConstant.AllowedFromDateTimeSql
                    && dateTime <= DateConstant.AllowedToDateTimeSql);
        }
    }
}