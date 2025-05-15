using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using BookingAds.Services.Payment;

namespace BookingAds.Constants
{
    public static class ConfigVnpayConstant
    {
        public static string ReturnUrl = ConfigurationManager.AppSettings["vnp_Returnurl"];
        public static string Url = ConfigurationManager.AppSettings["vnp_Url"];
        public static string TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
        public static string HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
        public static string Version = VnPayLibrary.VERSION;
        public const string Command = "pay";
        public const string BankCode = "VNBANK";
        public const string OrderType = "other";
        public static string CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        public const string CurrCode = "VND";
        public static string IpAddr = Utils.GetIpAddress();
        public const string Locale = "vn";
        public const string Successed = "00";
    }
}