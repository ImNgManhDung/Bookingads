using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Supports
    {
        public long SupportsID { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }

        public string SubjectMesseger { get; set; }

        public string ImageRequest { get; set; }

        public DateTime TimeSend { get; set; }

        public int Status { get; set; }

        public Employee Employee { get; set; }

        public Product Service { get; set; }

        public CatelogProduct CatelogProduct { get; set; }
    }
}