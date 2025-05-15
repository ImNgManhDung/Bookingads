using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Message
{
    public class ViewEditMessage
    {
        public long MessageID { get; set; }

        public string SenderID { get; set; }

        public string Content { get; set; }
    }
}