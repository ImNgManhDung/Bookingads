using System.Collections.Generic;
using BookingAds.Areas.Admin.Models.ManageOrder;

namespace BookingAds.Areas.Admin.Repository.Order.Abstractions
{
    using BookingAds.Entities;

    public interface IOrderRepository
    {
        IReadOnlyList<Order> GetOrders(ViewFilterOrder viewData);

        int Count(ViewFilterOrder viewData);

        Order GetOrder(long orderID);

        bool Delete(long orderID);

        bool DeleteAllOrdersAreRejectedOrCanceled();

        bool Approve(long orderID);

        bool Reject(long orderID);

        bool AcceptGotProduct(long orderID, string link);

        bool AcceptPayed(long orderID);

        bool AcceptNotPay(long orderID);
        //Order GetOrder(long oderID, long employeeID);

        IReadOnlyList<OderDetailMess> LoadDetailMess(long oderID);
        int CreateDetailMess(ViewDetailsMess dataDto);
        int GetPhone(long orderID);
    }
}
