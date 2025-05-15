using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Modules
{
    public static class PasswordUtils
    {
        public static string MD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower().Substring(0, 20);
            }
        }
    }
}