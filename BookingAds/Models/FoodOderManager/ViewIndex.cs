using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;

namespace BookingAds.Models.ProductOderManager
{
    public class ViewIndex
    {
           public IReadOnlyList<Order> Order { get; set; }
    }
}