using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;

namespace BookingAds.Areas.Admin.Models.ManageEmployee
{
    public class ViewFilterEmployee : ViewPaginateInputBase
    {
        public int Gender { get; set; }

        public int Field { get; set; }

        public string SearchValue { get; set; }
    }
}