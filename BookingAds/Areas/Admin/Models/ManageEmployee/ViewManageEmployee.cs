using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;
using BookingAds.Entities;

namespace BookingAds.Areas.Admin.Models.ManageEmployee
{
    public class ViewManageEmployee : ViewPaginateOutputBase<Employee>
    {
        public string SearchValue { get; set; }

        public int Gender { get; set; }

        public int Field { get; set; }

        // public IReadOnlyList<TestModel> DataTest { get; set; }
    }
}