using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingAds.Models.InformationBookingAds
{
    public class ViewRecharge
    {
        public string EmployeeUserName { get; set; }

        public string ContentPay { get; set; }

        public long Money { get; set; }
    }
}