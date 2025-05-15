using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;

namespace BookingAds.Models.InformationBookingAds
{
    public class ViewDetails
    {
        public IReadOnlyList<Product> Product { get; set; }
    }
}