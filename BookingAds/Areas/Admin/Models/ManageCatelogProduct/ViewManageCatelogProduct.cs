using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;
using BookingAds.Entities;

namespace BookingAds.Areas.Admin.Models.ManageCatelogProduct
{
    public class ViewManageCatelogProduct : ViewPaginateOutputBase<CatelogProduct>
    {
        public string SearchValue { get; set; }

        public ViewCreateCatelogProduct DataDto { get; set; }
    }
}