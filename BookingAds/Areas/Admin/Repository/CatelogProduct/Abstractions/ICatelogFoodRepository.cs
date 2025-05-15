using System.Collections.Generic;
using BookingAds.Areas.Admin.Models.ManageCatelogProduct;

namespace BookingAds.Areas.Admin.Repository.CatelogProduct.Abstractions
{
    using BookingAds.Entities;

    public interface ICatelogProductRepository
    {
        IReadOnlyList<CatelogProduct> GetCatelogProducts();

        IReadOnlyList<CatelogProduct> LoadCatelogProducts(ViewFilterCatelogProduct viewData);

        int Count(ViewFilterCatelogProduct viewData);

        CatelogProduct GetCatelogProducts(long id);

        bool CheckCreateCatelogProduct(string catelogName);

        bool CreateCatelogProduct(ViewCreateCatelogProduct dataDto);

        bool UpdateCatelogProduct(ViewCreateCatelogProduct dataDto);

        bool DeleteCatelogProduct(long catelogID);
    }
}
