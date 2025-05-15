using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Message
{
    public class ViewReadMessage
    {
        public string SenderID { get; set; }

        public string ReceiverID { get; set; }

        public string ReadTime { get; set; }
    }
}