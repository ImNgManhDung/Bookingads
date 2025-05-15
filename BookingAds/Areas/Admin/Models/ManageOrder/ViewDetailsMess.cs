using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Areas.Admin.Models.ManageOrder
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