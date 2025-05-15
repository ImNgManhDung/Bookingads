using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class MessTest
    {
        public int MessageID { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}