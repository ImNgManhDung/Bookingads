using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Base
{
    public class ViewPaginateInputBase
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}