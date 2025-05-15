using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Repository.SelectListHelper.Abstractions
{
    internal interface ISelectListHelperRepository
    {
        IReadOnlyList<Order> ListServices_Customer(long customer);
    }
}