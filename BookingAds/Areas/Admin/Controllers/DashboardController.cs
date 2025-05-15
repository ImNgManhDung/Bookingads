using System.Globalization;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.Dashboard;
using BookingAds.Areas.Admin.Repository.Dashboard;
using BookingAds.Areas.Admin.Repository.Dashboard.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Constants;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class DashboardController : Controller
    {
        #region Repository
        private readonly IDashboardRepository _dashboardRepo = new DashboardRepository();
        #endregion
        #region Constant
        private const string AreaCurrency = "vi-VN";
        private const string AreaCurrencyFormat = "C";
        private const string ControllerName = "Dashboard";
        private const string ActionIndex = "Index";
        private const string ActionTopThreeProductStatistic = "TopThreeProductStatistic";
        private const string ActionRevenueOfCurrentMonthStatistic = "RevenueOfCurrentMonthStatistic";
        #endregion
        #region Action

        // GET: Admin/Dashboard
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var viewData = new ViewIndexDashboard()
            {
                CountEmployee = _dashboardRepo.CountEmployee(),
                CountProduct = _dashboardRepo.CountProduct(),
                CountOrder = _dashboardRepo.CountOrder(),
                CountRevenue = _dashboardRepo.CountRevenue().ToString(
                    AreaCurrencyFormat,
                    CultureInfo.GetCultureInfo(AreaCurrency)),
            };

            return View(viewData);
        }

        // GET: Admin/Dashboard/TopThreeProductStatistic
        [HttpGet]
        [ActionName(ActionTopThreeProductStatistic)]
        public ActionResult TopThreeProductStatistic()
        {
            var data = _dashboardRepo.TopThreeProductStatistic();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Dashboard/RevenueOfCurrentMonthStatistic
        [HttpGet]
        [ActionName(ActionRevenueOfCurrentMonthStatistic)]
        public ActionResult RevenueOfCurrentMonthStatistic()
        {
            var data = _dashboardRepo.RevenueOfCurrentMonthStatistic();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}