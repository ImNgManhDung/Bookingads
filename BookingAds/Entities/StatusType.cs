namespace BookingAds.Entities
{
    public class StatusType
    {
        public int Code { get; }

        public string Message { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<Pending>")]
        public StatusType(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public static string ToFormat(int statusCode)
        {
            switch (statusCode)
            {

                case -2:
                case -1:
                    return "text-muted text-center";
                case 0:
                    return "text-primary text-center";
                case 1:
                case 2:
                    return "text-warning text-center";
                case 99:
                    return "text-success text-center";
                case -99:
                    return "text-danger text-center";

            }

            return string.Empty;
        }

        public static string ToMessage(int statusCode)
        {
            switch (statusCode)
            {

                case -2:
                    return "Đơn hàng bị hủy";
                case -1:
                    return "Đơn hàng bị từ chối";
                case 0:
                    return "Đang xử lý";
                case 1:
                    return "Đã duyệt đơn. Chờ hoàn thành";
                case 2:
                    return "Sản phẩm đang trong quá trình hoàn thânh";
                case 99:
                    return "Đã hoàn thành dự án";
                case -99:
                    return "Không được thanh toán. Đơn hàng đã đặt thất bại";

            }

            return string.Empty;
        }
    }
}