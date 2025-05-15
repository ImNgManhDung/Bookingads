using System;
using System.Threading.Tasks;
using BookingAds.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BookingAds.Startup))]

namespace BookingAds
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var userNameProvider = new UserNameProvider();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userNameProvider);

            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
