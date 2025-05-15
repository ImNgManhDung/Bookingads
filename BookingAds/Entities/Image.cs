using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Image
    {
        public int ImageID { get; set; }
        public int MessageID { get; set; }
        public string ImagePath { get; set; }
        public Message Message { get; set; }
    }
}