using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Areas.Admin.Models.Dashboard
{
    public class ViewIndexDashboard
    {
        public int CountEmployee { get; set; }

        public int CountProduct { get; set; }

        public int CountOrder { get; set; }

        public string CountRevenue { get; set; }
    }
}