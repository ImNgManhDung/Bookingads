using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public static class OrderStatus
    {
        public const int DEFAULT = 69;

        public static StatusType CANCELED = new StatusType(-2, "Đơn hàng bị hủy");

        public static StatusType REJECTED = new StatusType(-1, "Đơn hàng bị từ chối");

        public static StatusType PENDING = new StatusType(0, "Đang xử lý");

        public static StatusType WAITING = new StatusType(1, "Đã duyệt đơn. Chờ hoàn thành");

        public static StatusType PAYING = new StatusType(2, "Sản phẩm đang trong quá trình hoàn thânh");

        public static StatusType SUCCESSED = new StatusType(99, "Đã hoàn thành dự án");

        public static StatusType FAILED = new StatusType(-99, "Không được thanh toán. Đơn hàng đã đặt thất bại");
    }
}