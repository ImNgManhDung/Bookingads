using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Models.Supports
{
    using Entities;

    public class ViewDetails
    {
        public Supports Supports { get; set; }

        public IReadOnlyList<SupportsDetails> SupportsDetail { get; set; }

        public string Messenger { get; set; }

        public string Images { get; set; }
    }
}