using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class OderDetailMess
    {
        public string SenderID { get; set; }

        public string ReceiverID { get; set; }

        public Content Content { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime ReadTime { get; set; }

        public long OrderID { get; set; }

        public long OderDetailMessID { get; set; }
    }
}