using System.Web.Mvc;
using System.Web.Security;
using BookingAds.Areas.Admin.Repository.Product;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Areas.Admin.Repository.Order;
using BookingAds.Areas.Admin.Repository.Order.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Models.HistoryOrderProduct;
using BookingAds.Modules;
using BookingAds.Repository.HistoryOrderProduct;
using BookingAds.Repository.HistoryOrderProduct.Abstractions;
using System.Web;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class HistoryOrderProductController : Controller
    {
        #region Repository
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        private readonly IHistoryOrderProductRepository _historyOrderProductRepo = new HistoryOrderProductRepository();
        private readonly IOrderRepository _orderRepo = new OrderRepository();
        private readonly IProductRepository _productRepo = new ProductRepository();
        #endregion
        #region Constant
        private const int PAGE_SIZE = 10;
        private const string FormatDateTimeFilter = "yyyy-MM-ddTHH:mm";
        private const string ControllerName = "HistoryOrderProduct";
        private const string ActionIndex = "Index";
        private const string ActionSearchHistoryOrder = "SearchHistoryOrder";
        private const string ActionProducts = "Products";
        private const string ActionEdit = "Edit";
        private const string ActionCanceled = "Canceled";
        #endregion
        #region Action

        // GET: HistoryOrderProduct
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            ViewBag.Title = "Lịch sử đặt quảng cáo";
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);

            var viewData = new ViewFilterHistory()
            {
                OrderStatus = OrderStatus.DEFAULT,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
                DateStart = string.Empty,
                DateEnd = string.Empty,
            };

            var model = new ViewIndex()
            {
                DateStart = viewData.DateStart,
                DateEnd = viewData.DateEnd,
                OrderStatus = viewData.OrderStatus,
                SearchValue = viewData.SearchValue,
                Data = _historyOrderProductRepo.LoadHistoryOrderProduct(viewData, currentEmployee.EmployeeID),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _historyOrderProductRepo.Count(viewData, currentEmployee.EmployeeID),
            };

            return View(model);
        }

        // Post: HistoryOrderProduct/SearchHistoryOrder
        [HttpPost]
        [ActionName(ActionSearchHistoryOrder)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult SearchHistoryOrder(ViewFilterHistory viewData)
        {
            if (viewData == null)
            {
                return Json(ConstantsReturnMessengerOrder.ErrorNull, JsonRequestBehavior.AllowGet);
            }

            if (!DateTimeUtils.IsValidDateTimeSQL(viewData.DateStart)
               || !DateTimeUtils.IsValidDateTimeSQL(viewData.DateEnd))
            {
                return Json(ConstantsReturnMessengerOrder.InvalidDateTime, JsonRequestBehavior.AllowGet);
            }

            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);

            var model = new ViewIndex()
            {
                DateEnd = viewData.DateEnd,
                DateStart = viewData.DateStart,
                OrderStatus = viewData.OrderStatus,
                SearchValue = viewData.SearchValue,
                Data = _historyOrderProductRepo.LoadHistoryOrderProduct(viewData, currentEmployee.EmployeeID),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _historyOrderProductRepo.Count(viewData, currentEmployee.EmployeeID),
            };

            return View(model);
        }

        // POST: HistoryOrderProduct/Products
        [HttpPost]
        [ActionName(ActionProducts)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Products(string searchValue = "")
        {
            var products = _historyOrderProductRepo.GetProducts(searchValue);

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        // POST: HistoryOrderProduct/Edit
        [HttpPost]
        [ActionName(ActionEdit)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Edit(long productId = 0, long orderId = 0)
        {
            if (productId == 0 || orderId == 0)
            {
                return Json(ConstantsReturnMessengerOrder.ErrorNull, JsonRequestBehavior.AllowGet);
            }

            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            var employeeID = currentEmployee.EmployeeID;
            var order = _orderRepo.GetOrder(orderId);

            if (order == null)
            {
                return Json(ConstantsReturnMessengerOrder.OrderIsNotFound, JsonRequestBehavior.AllowGet);
            }

            if (order.Status != OrderStatus.PENDING.Code)
            {
                return Json(ConstantsReturnMessengerOrder.NotHandle, JsonRequestBehavior.AllowGet);
            }

            // check current balance bigger than product price
            var productInfo = _productRepo.GetProduct(productId);
            var prevBalance = order.TotalMoney + currentEmployee.Coin;

            if (prevBalance < productInfo.Price)
            {
                return Json(ConstantsReturnMessengerOrder.NotEnoughMoney, JsonRequestBehavior.AllowGet);
            }

            var newBalance = prevBalance - productInfo.Price;
            _historyOrderProductRepo.UpdateCoin(employeeID, newBalance);

            bool editOrder = _historyOrderProductRepo.EditOrder(employeeID, productId, orderId);
            if (!editOrder)
            {
                return Json(ConstantsReturnMessengerOrder.ErrorNull, JsonRequestBehavior.AllowGet);
            }

            var employee = _accountRepo.GetEmployee(currentEmployee.UserName);
            var newCurrentEmployee = ConvertUtils<Employee>.Serialize(employee);
            FormsAuthentication.SignOut();
            FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);

            return Json(ConstantsReturnMessengerOrder.SuccessOrder, JsonRequestBehavior.AllowGet);
        }

        // POST: HistoryOrderProduct/Canceled
        [HttpPost]
        [ActionName(ActionCanceled)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Canceled(long orderId = 0)
        {
            if (orderId == 0)
            {
                return Json(ConstantsReturnMessengerOrder.ErrorNull, JsonRequestBehavior.AllowGet);
            }

            var order = _orderRepo.GetOrder(orderId);

            if (order.Status == OrderStatus.PENDING.Code)
            {
                var cancel = _historyOrderProductRepo.Cancel(order.OrderID);
                if (!cancel)
                {
                    return Json(ConstantsReturnMessengerOrder.FaildCancelOrder, JsonRequestBehavior.AllowGet);
                }

                if (order.Type == PayTypeConstant.WALLET)
                {
                    var newBalance = order.TotalMoney + order.Employee.Coin;
                    _historyOrderProductRepo.UpdateCoin(order.Employee.EmployeeID, newBalance);
                    var employee = _accountRepo.GetEmployee(order.Employee.UserName);
                    var newCurrentEmployee = ConvertUtils<Employee>.Serialize(employee);
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);
                }
            }

            return Json(ConstantsReturnMessengerOrder.CancelOrder, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("DetailsMess")]
        [AllowAnonymous]
        public ActionResult DetailsMess(long orderID)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (orderID.Equals(null))
            {
                return RedirectToAction(actionName: "Index", controllerName: ControllerName);
            }

            var model = new ViewDetailsMess()
            {
                Order = _historyOrderProductRepo.GetOrder(orderID, currentEmployee.EmployeeID),
                DetailMess = _historyOrderProductRepo.LoadDetailMess(orderID, currentEmployee.EmployeeID),
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("DetailsMess")]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsMess(ViewDetailsMess dataDto, IEnumerable<HttpPostedFileBase> photoUploaded, string chatInput, long orderID)
        {
            string rootPath = Server.MapPath("~/Images/HistoryOrder/MessImgDetails/");
            List<string> savedFiles = new List<string>();

            if (string.IsNullOrWhiteSpace(chatInput) || photoUploaded == null)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập.";
                return RedirectToAction("DetailsMess", "HistoryOrderProduct", new { orderId = orderID });
            }


            foreach (var file in photoUploaded)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!UploadFileConstant.AllowedExtensions.Contains(fileExtension)
                        || UploadFileConstant.RejectExtensions.Contains(fileExtension))
                    {
                        TempData["ErrorMessage"] = "Định dạng tệp không được phép.";
                        return RedirectToAction("DetailsMess", "HistoryOrderProduct", new { orderId = orderID });
                    }

                    var uniqueFileName = $"{DateTime.Now.Ticks}_{file.FileName}";

                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }

                    var path = Path.Combine(rootPath, uniqueFileName);
                    file.SaveAs(path);
                    savedFiles.Add(uniqueFileName);
                }
            }

            string fileZip = string.Join(",", savedFiles);

            // Save file names to dataDto.Images
            if (savedFiles.Any())
            {
                dataDto.Images = fileZip;
            }
            dataDto.OrderId = orderID;
            dataDto.Messenger = chatInput;

            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
             var isCreated = _historyOrderProductRepo.CreateDetailMess(dataDto, currentEmployee.EmployeeID);

            if (isCreated <= 0)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm chi tiết hỗ trợ.";
                return RedirectToAction("DetailsMess", "HistoryOrderProduct", new { orderId = orderID });
            }
           

            return RedirectToAction("DetailsMess", "HistoryOrderProduct", new { orderId = orderID });
        }

        #endregion
    }
}
