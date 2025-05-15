using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Entities
{
    public class Employee : Account
    {
        public long EmployeeID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Gender { get; set; }

        public string Avatar { get; set; }

        public DateTime LockedAt { get; set; }

        public long Coin { get; set; }

        public string Phone { get; set; }
    }
}