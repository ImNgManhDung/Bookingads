using System;
using System.Text;

namespace BookingAds.Modules
{
    public static class GenTokenUtils
    {
        public static string GenerateToken(int? length)
        {
            // CA2208 : ArgumentException => ArgumentOutOfRangeException
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            char[] charArray = @"abcdefghijknmpqrstuvwxyzABCDEFGHJKLNMPQRSTUVWXYZ123456789".ToCharArray();
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            var tokenDB = string.Empty;
            for (int index = 0; index < length; ++index)
            {
                stringBuilder.Append(charArray[random.Next(charArray.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}
