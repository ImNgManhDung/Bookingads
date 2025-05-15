using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAds.Controllers
{
    public class AdsPageController : Controller
    {
        // GET: AdsPage
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdsPage/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdsPage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdsPage/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdsPage/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdsPage/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdsPage/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdsPage/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
