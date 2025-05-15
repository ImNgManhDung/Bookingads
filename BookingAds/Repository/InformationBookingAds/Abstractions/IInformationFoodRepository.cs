using System.Collections.Generic;
using BookingAds.Models.InformationBookingAds;

namespace BookingAds.Repository.InformationBookingAds.Abstractions
{
    using BookingAds.Entities;

    internal interface IInformationBookingAdsRepository
    {
        IReadOnlyList<Product> LoadInfomationsProduct();

        IReadOnlyList<Product> LoadInfomationsProduct(long id);

        IReadOnlyList<Product> GetProducts(ViewFilterBookingAds viewData);

        int Count(ViewFilterBookingAds viewData);

        bool Recharge(string employeeUserName, long money);

        IReadOnlyList<ProductAttributes> GetProductAttributes(long productId);
        IReadOnlyList<ProductDescription> GetProductDescription(long productId);
    }
}