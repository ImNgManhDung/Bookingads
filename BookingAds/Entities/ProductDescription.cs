using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class ProductDescription : Product
    {
        public string DescriptionID { get; set; }

        public string DescriptionDetail { get; set; }
    }
}