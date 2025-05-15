using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;

namespace BookingAds.Models.Supports
{
    public class ViewFilterSupports : ViewPaginateInputBase
    {
        public int OrderStatus { get; set; }

        public string SearchValue { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }
    }
}