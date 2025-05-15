using System.Collections.Generic;
using BookingAds.Areas.Admin.Models.ManageProduct;

namespace BookingAds.Areas.Admin.Repository.Product.Abstractions
{
    using BookingAds.Entities;

    internal interface IProductRepository
    {
        IReadOnlyList<Product> GetProducts(ViewFilterBookingAds viewData);

        int Count(ViewFilterBookingAds viewData);

        Product GetProduct(long productID);

        bool CreateProduct(ViewCreateProduct dataDto);

        bool UpdateProduct(ViewCreateProduct dataDto);

        bool SoftDeleteProduct(long productID);

        bool RestoreProduct(long productID);
    }
}
