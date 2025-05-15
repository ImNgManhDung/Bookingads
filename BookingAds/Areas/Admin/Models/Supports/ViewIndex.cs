using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;

namespace BookingAds.Areas.Admin.Models.Supports
{
    using BookingAds.Entities;

    public class ViewIndex : ViewPaginateOutputBase<Supports>
    {
        public string SearchValue { get; set; }

        public int OrderStatus { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }
    }
}