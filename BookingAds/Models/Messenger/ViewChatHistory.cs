using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Models.Messenger
{
    using BookingAds.Entities;

    public class ViewChatHistory
    {
        public IReadOnlyList<Message> Messages { get; set; }
    }
}