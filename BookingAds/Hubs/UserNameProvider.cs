using BookingAds.Entities;
using BookingAds.Modules;
using Microsoft.AspNet.SignalR;

namespace BookingAds.Hubs
{
    /// <summary>
    /// Define method allow use username for chathub class instead of connection id.
    /// </summary>
    public class UserNameProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            var currentUserName = ConvertUtils<Account>.Deserialize(request.User.Identity.Name).UserName;
            return currentUserName;
        }
    }
}