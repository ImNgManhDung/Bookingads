using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Constants
{
    public static class DateConstant
    {
        public static DateTime DateStart = new DateTime(1999, 1, 1, 0, 0, 0);
        public static DateTime DateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        public static DateTime AllowedFromDateTimeSql = new DateTime(1753, 1, 1, 0, 0, 0);
        public static DateTime AllowedToDateTimeSql = new DateTime(9999, 12, 31, 0, 0, 0);
    }
}