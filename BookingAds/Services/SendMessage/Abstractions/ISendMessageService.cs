using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingAds.Services.Abstractions
{
    internal interface ISendMessageService
    {
        string SendMessage(string msg);
    }
}
