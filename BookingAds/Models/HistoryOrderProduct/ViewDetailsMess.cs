using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Models.HistoryOrderProduct
{
    public class ViewDetailsMess
    {
        public IReadOnlyList<OderDetailMess> DetailMess { get; set; }

        public Order Order { get; set; }

        public long OrderId { get; set; }

        public string Messenger { get; set; }

        public string Images { get; set; }
    }
}