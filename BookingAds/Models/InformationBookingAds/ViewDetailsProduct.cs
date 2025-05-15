using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Models.InformationBookingAds
{
    public class ViewDetailsProduct
    {
        public int PayType { get; set; }

        public IReadOnlyList<Product> Products { get; set; }

        public IReadOnlyList<ProductAttributes> ProductAttributes { get; set; }

        public IReadOnlyList<ProductDescription> ProductDescription { get; set; }
    }
}