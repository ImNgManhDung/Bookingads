using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;
using BookingAds.Entities;

namespace BookingAds.Models.HistoryOrderProduct
{
    public class ViewIndex : ViewPaginateOutputBase<Order>
    {
        public string SearchValue { get; set; }

        public int OrderStatus { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }
    }
}