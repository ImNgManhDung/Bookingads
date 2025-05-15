using System.Collections.Generic;
using BookingAds.Models.HistoryOrderProduct;

namespace BookingAds.Repository.HistoryOrderProduct.Abstractions
{
    using BookingAds.Entities;

    public interface IHistoryOrderProductRepository
    {
        IReadOnlyList<Order> LoadHistoryOrderProduct(ViewFilterHistory viewData, long employeeID);

        IReadOnlyList<Product> GetProducts(string searchValue);

        int Count(ViewFilterHistory viewData, long employeeID);

        bool EditOrder(long employeeID, long productID, long orderID);

        bool Cancel(long orderID);

        bool UpdateCoin(long employeeID, decimal newBalance);
        Order GetOrder(long oderID, long employeeID);
        IReadOnlyList<OderDetailMess> LoadDetailMess(long oderID, long employeeID);
        int CreateDetailMess(ViewDetailsMess dataDto, long employeeID);
    }
}