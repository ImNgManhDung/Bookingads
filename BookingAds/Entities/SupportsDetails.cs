using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class SupportsDetails : Supports
    {
        public string ReceiverID { get; set; }

        public string SenderID { get; set; }

        public string Image { get; set; }

        public string Messeger { get; set; }

        public DateTime TimeRep { get; set; }
    }
}