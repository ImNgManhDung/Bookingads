using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Repository.CatelogProduct;

namespace BookingAds.Modules
{
    public static class SelectListUtils
    {
        public static IEnumerable<SelectListItem> GetCatelogProducts()
        {
            var list = new List<SelectListItem>();

            // default value
            list.Add(
                new SelectListItem()
                {
                    Value = "0",
                    Text = "-- Loại quảng cáo --",
                });

            var catelogProducts = new CatelogProductRepository().GetCatelogProducts();
            foreach (var catelogProduct in catelogProducts)
            {
                list.Add(new SelectListItem()
                {
                    Value = catelogProduct.CatelogProductsID.ToString(),
                    Text = catelogProduct.CatelogName,
                });
            }

            return list;
        }
    }
}