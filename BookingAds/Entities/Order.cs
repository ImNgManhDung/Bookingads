using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Order
    {
        public long OrderID { get; set; }

        public Employee Employee { get; set; }

        public Product Product { get; set; }

        public CatelogProduct CatelogProduct { get; set; }

        public DateTime OrderedTime { get; set; }

        public int Status { get; set; }

        public string Textlink { get; set; }

        public int Type { get; set; }

        public long TotalMoney { get; set; }
    }
}