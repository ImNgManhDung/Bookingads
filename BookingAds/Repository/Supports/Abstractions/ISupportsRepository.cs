using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Entities;
using BookingAds.Models.Supports;

namespace BookingAds.Repository.Supports.Abstractions
{
    using BookingAds.Entities;
    using BookingAds.Models.Supports;

    internal interface ISupportsRepository
    {
        IReadOnlyList<Supports> LoadSupport(ViewFilterSupports viewData, long customerId);

        int Count(ViewFilterSupports viewData, long customerId);

        IReadOnlyList<SupportsDetails> LoadSupportDetail(long supportsID, long customerId);

        Supports GetSupports(long supportsID);

        int CreateSupports(ViewCreate dataview, long employeeID);
        int CreateSupportsDetail(ViewDetails dataDto, long employeeID);
    }
}