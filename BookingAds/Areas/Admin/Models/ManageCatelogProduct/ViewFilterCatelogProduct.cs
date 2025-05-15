using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;

namespace BookingAds.Areas.Admin.Models.ManageCatelogProduct
{
    public class ViewFilterCatelogProduct : ViewPaginateInputBase
    {
        public string SearchValue { get; set; }
    }
}