using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Common.Models.Base;
using BookingAds.Entities;

namespace BookingAds.Models.InformationBookingAds
{
    public class ViewIndex : ViewPaginateOutputBase<Product>
    {
        public IReadOnlyList<Product> Product { get; set; }

        public int PayType { get; set; }

        public string Sort { get; set; }

        public string SortField { get; set; }

        public int SortType { get; set; }

        public int CatelogProductsID { get; set; }

        public string SearchValue { get; set; }
    }
}