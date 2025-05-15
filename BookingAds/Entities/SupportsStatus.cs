using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public static class SupportsStatus
    {
        public const int DEFAULT = 69;

        public static SupportsType RequestSend = new SupportsType(0, "Yêu Cầu Đã Gửi");

        public static SupportsType RequestForFeedback = new SupportsType(1, "Yêu Cầu Đã Có Phản Hồi");

        public static SupportsType RequestDone = new SupportsType(2, "Yêu Cầu Đã Hoàn Thành");
    }
}