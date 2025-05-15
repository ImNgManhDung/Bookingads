using System.Collections.Generic;

namespace BookingAds.Repository.InformationBookingAds.Abstractions
{
    using BookingAds.Entities;

    internal interface IOrderProductRepository
    {
        int OrderProduct(int employeeID, int productID, int payType);

        bool CheckCoin(int employeeID, int productID);

        bool CheckProductIsLocked(int productID);

        void DeductionCoin(int employeeID, int productID);

        Order GetOrder(long orderID);
    }
}