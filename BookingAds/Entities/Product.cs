using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Product
    {
        public long ProductID { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsLocked { get; set; }

        public CatelogProduct CatelogProduct { get; set; }

        public string Photo { get; set; }
    }
}