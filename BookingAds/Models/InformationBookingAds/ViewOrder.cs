using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;

namespace BookingAds.Models.InformationBookingAds
{
    public class ViewOrder
    {
        public IReadOnlyList<Order> OrderProduct { get; set; }

        public IReadOnlyList<Employee> Employees { get; set; }
    }
}