using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BookingAds.Entities;
using BookingAds.Modules;
using Microsoft.AspNet.SignalR;

namespace BookingAds.Hubs
{
    /// <summary>
    /// The chat hub class contain logic chat real-time.
    /// </summary>
    public class ChatHub : Hub
    {
        private static IList<string> _onlines = new List<string>();

        public override Task OnConnected()
        {
            var currentUser = ConvertUtils<Account>.Deserialize(Context.User.Identity.Name);
            if (!_onlines.Contains(currentUser.UserName))
            {
                _onlines.Add(currentUser.UserName);
            }

            Clients.All.Online(_onlines);
            return base.OnConnected();
        }

        public void Send(string msg)
        {
            Clients.All.SendMsg(msg);
        }

        /// <summary>
        /// Define send private one to one method.
        /// </summary>
        /// <param name="myUserName">The senderID is my username.</param>
        /// <param name="message">The content is text.</param>
        /// <param name="otherUserName">The receiverID is other username.</param>
        public void SendPrivate(string myUserName, string message, string otherUserName)
        {
            var data = new
            {
                MyUserName = myUserName,
                OtherUserName = otherUserName,
                CreatedTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
                Content = message,
            };

            Clients.User(myUserName).SendPrivateMsg(data);
            Clients.User(otherUserName).SendPrivateMsg(data);
        }

        /// <summary>
        /// Define read message private method.
        /// </summary>
        /// <param name="myUserName">The senderID is my username.</param>
        /// <param name="otherUserName">The receiverID is other username.</param>
        public void ReadMessagePrivate(string myUserName, string otherUserName)
        {
            var data = new
            {
                MyUserName = myUserName,
                OtherUserName = otherUserName,
                ReadTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
            };

            Clients.User(myUserName).ReadMessagePrivateMsg(data);
            Clients.User(otherUserName).ReadMessagePrivateMsg(data);
        }

        public void Online()
        {
            Clients.All.Online(_onlines);
        }

        public void Offline(string currentUserName)
        {
            if (_onlines.Contains(currentUserName))
            {
                _onlines.Remove(currentUserName);
                Clients.All.Online(_onlines);
            }
        }
    }
}