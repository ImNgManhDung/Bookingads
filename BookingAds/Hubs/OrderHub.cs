using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BookingAds.Entities;
using BookingAds.Modules;
using Microsoft.AspNet.SignalR;

namespace BookingAds.Hubs
{
    public class OrderHub : Hub
    {
        public void NotifyOrder(string fromUserName, string notificationData, IList<string> toUserNames)
        {
            var data = new
            {
                FromUserName = fromUserName,
                NotificationData = notificationData,
                ToUserNames = toUserNames,
            };

            Clients.Users(toUserNames).NotifyOrderMsg(data);
        }
    }
}