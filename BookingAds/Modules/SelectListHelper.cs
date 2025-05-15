using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Common.Repository.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Repository.SelectListHelper.Abstractions;
using BookingAds.Repository.Supports;
using BookingAds.Repository.SelectListHelper;
using BookingAds.Entities;

namespace BookingAds.Modules
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> ListService(long customerId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "-99",
                Text = "-- Chọn loại dịch vụ --",
            });

            foreach (var item in SelectListHelperRepository.ListServices_Customer(customerId))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.OrderID.ToString(),
                    Text = $" {item.Product.ProductName} ( {item.CatelogProduct.CatelogName} ) | {item.OrderedTime}  ",
                });
            }

            return list;
        }
    }

}
