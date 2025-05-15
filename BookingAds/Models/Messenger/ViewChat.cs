using System.Collections.Generic;

namespace BookingAds.Models.Messenger
{
    using BookingAds.Entities;

    public class ViewChat
    {
        public IReadOnlyList<Admin> Admins { get; set; }
    }
}