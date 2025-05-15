using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Services.SendMail.Abstractions
{
     public interface ISendMailService
    {
        void SendMail(string mailBody, string mailTo, string subject, bool isHtml = false);

        void SendMail(string mailBody, string[] mailTo, string subject, bool isHtml = false);
    }
}