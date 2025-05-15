using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;

namespace BookingAds.Areas.Admin.Models.ManageEmployee
{
    public class ViewChat
    {
        public IReadOnlyList<Employee> Employees { get; set; }
    }
}