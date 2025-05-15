using System.Web;
using System.Web.Mvc;
using BookingAds.Attributes.Filters;

namespace BookingAds
{
    // CA1053 : add static
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new HandleAntiforgeryTokenErrorAttribute()
                { ExceptionType = typeof(HttpAntiForgeryException) });
        }
    }
}
