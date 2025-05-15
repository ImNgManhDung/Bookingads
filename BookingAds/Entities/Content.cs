using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Content : OderDetailMess
    {
        public int ContentID { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }
    }
}