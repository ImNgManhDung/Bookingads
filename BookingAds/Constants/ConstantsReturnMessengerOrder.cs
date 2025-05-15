using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Constants
{
    public static class ConstantsReturnMessengerOrder
    {
        public const string NotEnoughMoney = "Tài khoản không đủ tiền";

        public const string SuccessOrder = "";

        public const string CancelOrder = "Huỷ đặt món thành công";

        public const string FaildCancelOrder = "Huỷ đặt món thành công";

        public const string NotHandle = "Có vẻ như đơn của bạn không nằm trong trạng thái Đang xử lý";

        public const string ErrorNull = "Lỗi";

        public const string InvalidDateTime = "Lọc theo thời điểm không hợp lệ";

        public const string ProductIsLocked = "Quảng cáo không tồn tại";

        public const string OrderIsNotFound = "Đơn hàng không tồn tại";
    }
}