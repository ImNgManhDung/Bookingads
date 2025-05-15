using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class SupportsType
    {
        public int Code { get; }

        public string Message { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<Pending>")]
        public SupportsType(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public static string ToFormat(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return "text-primary text-center";
                case 1:
                    return "text-warning text-center";
                case 2:
                    return "text-success text-center";

            }

            return string.Empty;
        }

        public static string ToMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return "Yêu Cầu Đã Gửi";
                case 1:
                    return "Yêu Cầu Đã Có Phản Hồi";
                case 2:
                    return "Yêu Cầu Đã Hoàn Thành";
            }

            return string.Empty;
        }
    }
}