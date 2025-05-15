using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Message
    {
        public string SenderID { get; set; }

        public string ReceiverID { get; set; }

        public string Content { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime ReadTime { get; set; }

        public long MessageID { get; set; }
    }
}