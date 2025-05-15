using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Common.Repository.Account;
using BookingAds.Entities;
using BookingAds.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using BookingAds.Common.Models.Account;
using Microsoft.Win32;
using BookingAds.Models.Register;
using BookingAds.Constants;

namespace BookingAds.Controllers
{
    public class RegisterController : Controller
    {
        #region Repository
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        #endregion
        #region Information Product

        private const string KeyErrorRegister = "Failed Register";
        private const string ControllerInformationBookingAds = "InformationBookingAds";
        private const string ActionIndexInformationBookingAds = "Index";
        private const string ControllerLogin = "Account";
        private const string ActionLogin = "Login";
        #endregion

        // GET: Register
        [HttpGet]
        public ActionResult Index()
        {
            var currentEmployeeIsExisted = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployeeIsExisted != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            return View();
        }

        // GET: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ViewRegister dataDto)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            var currentEmployeeIsExisted = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployeeIsExisted != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            var employee = _accountRepo.Register(dataDto);

            if (employee == false)
            {
                ModelState.AddModelError(KeyErrorRegister, ConstantsReturnMessengerAccount.ErrorLogin);
                return View(dataDto);
            }

            return RedirectToAction(ActionLogin, ControllerLogin);
        }
    }
}