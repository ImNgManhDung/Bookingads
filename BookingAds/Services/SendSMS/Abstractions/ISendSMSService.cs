using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingAds.Services.Abstractions
{
    internal interface ISendSMSService
    {
        string SendSMS(string phone, string msg);
    }
}
