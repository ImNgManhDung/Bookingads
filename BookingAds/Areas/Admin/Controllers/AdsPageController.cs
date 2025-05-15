using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAds.Areas.Admin.Controllers
{
    public class AdsPageController : Controller
    {
        // GET: Admin/AdsPage
        public ActionResult Index()
        {
            return View();
        }
    }
}