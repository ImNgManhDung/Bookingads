using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;

namespace BookingAds.Areas.Admin.Models.ManageProduct
{
    public class ViewFilterBookingAds : ViewPaginateInputBase
    {
        public string SortField { get; set; }

        public int SortType { get; set; }

        public int CatelogProductsID { get; set; }

        public string SearchValue { get; set; }
    }
}