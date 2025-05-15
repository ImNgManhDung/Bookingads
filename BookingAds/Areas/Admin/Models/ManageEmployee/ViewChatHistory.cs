using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;

namespace BookingAds.Areas.Admin.Models.ManageEmployee
{
    public class ViewChatHistory
    {
        public IReadOnlyList<Message> Messages { get; set; }
    }
}