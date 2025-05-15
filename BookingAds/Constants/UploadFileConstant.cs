using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Constants
{
    public static class UploadFileConstant
    {
        public static string[] AllowedExtensions = { ".png", ".jpg", ".gif", ".jpeg", ".tiff" , ".mp4" };
        public static string[] RejectExtensions = { ".exe", ".bat", ".cmd", ".vbs", ".js", ".php", ".asp", ".aspx", ".dll", ".zip", ".rar", ".tar", ".gz", ".mdb", ".db", ".config", ".ini", ".sh", ".ps1" };
    }
}