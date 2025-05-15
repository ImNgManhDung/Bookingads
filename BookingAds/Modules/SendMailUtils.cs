using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Services.SendMail;
using BookingAds.Services.SendMail.Abstractions;

namespace BookingAds.Modules
{
    public static class SendMailUtils
    {
        public static void SendMail(string mailTo, Uri callbackUrl, string authenCode)
        {
            string mailBody = $@"
Bạn đã yêu cầu cấp lại mật khẩu 

Đây là mã xác nhận ：{authenCode}

Hoặc có thể nhấn trực tiếp vào link này
{callbackUrl}

================================================";

            var subject = "BookingAds Code";
            var sendMail = new SendMailService();
            sendMail.SendMail(mailBody, mailTo, subject);
        }
    }
}