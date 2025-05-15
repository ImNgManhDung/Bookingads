using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Areas.Admin.Repository.Supports.Abstractions
{
    using BookingAds.Areas.Admin.Models.Supports;
    using BookingAds.Entities;

    internal interface ISupportsRepository
    {
        IReadOnlyList<Supports> LoadSupport(ViewFilterSupports viewData);

        int Count(ViewFilterSupports viewData);

        IReadOnlyList<SupportsDetails> LoadSupportDetail(long supportsID);

        Supports GetSupports(long supportsID);

        int CreateSupports(ViewCreate dataview);
        int CreateSupportsDetail(ViewDetails dataDto);
    }
}