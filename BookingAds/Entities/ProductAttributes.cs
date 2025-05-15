using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class ProductAttributes : Product
    {
        public long AttributeID { get; set; }

        public string AttributeName { get; set; }
    }
}