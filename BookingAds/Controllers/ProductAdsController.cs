using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAds.Controllers
{
    public class ProductAdsController : Controller
    {
        // GET: ProductAds
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string s)
        {
            return View();
        }

        // GET: ProductAds
        [HttpGet]
        public ActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Details(string s)
        {
            return View();
        }

    }
}