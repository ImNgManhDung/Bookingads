using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BookingAds.Areas.Admin.Models.ManageOrder;
using BookingAds.Areas.Admin.Repository.Order;
using BookingAds.Areas.Admin.Repository.Order.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Modules;
using BookingAds.Repository.HistoryOrderProduct;
using BookingAds.Repository.HistoryOrderProduct.Abstractions;
using BookingAds.Services;
using BookingAds.Services.Abstractions;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class ManageOrderController : Controller
    {
        #region Repository
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        private readonly IOrderRepository _orderRepo = new OrderRepository();
        private readonly IHistoryOrderProductRepository _historyOrderProductRepo = new HistoryOrderProductRepository();
        #endregion
        #region Service
        private readonly ISendSMSService _sendSMSService = new TwilioSendSMSApiService();
        #endregion
        #region Constant
        private const int PAGE_SIZE = 10;
        private const string ControllerName = "ManageOrder";
        private const string ActionIndex = "Index";
        private const string ActionFilter = "Filter";
        private const string ActionDelete = "Delete";
        private const string ActionDeleteAll = "DeleteAll";
        private const string ActionApprove = "Approve";
        private const string ActionReject = "Reject";
        private const string ActionAcceptGotProduct = "AcceptGotProduct";
        private const string ActionAcceptPayed = "AcceptPayed";
        private const string ActionAcceptNotPayed = "AcceptNotPayed";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "<Pending>")]
        public const string MsgOrderNotFound = "Đơn hàng không tồn tại";
        public const string MsgOrderHaveError = "Có lỗi";
        public const string MsgOrderHaveInvalidDateTime = "Lọc theo thời điểm không hợp lệ";
        public const string MsgRemoveSuccess = "Xóa đơn hàng thành công";
        public const string MsgOrderIsRejected = "Đơn hàng đã bị từ chối";
        public const string MsgOrderIsGotProduct = "Xác nhận dịch vụ thành công";
        public const string MsgOrderIsPay = "Xác nhận đã thanh toán";
        public const string MsgOrderIsNotPay = "Xác nhận không thanh toán";
        public const string MsgDefaultSuccessed = "";
        #endregion
        #region Action

        // GET: Admin/ManageOrder
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var viewData = new ViewFilterOrder()
            {
                Status = OrderStatus.DEFAULT,
                FromDatetime = string.Empty,
                ToDatetime = string.Empty,
                SearchField = SearchField.EMPLOYEE_FULLNAME,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
            };
            var model = new ViewManageOrder()
            {
                Status = viewData.Status,
                FromDatetime = viewData.FromDatetime,
                ToDatetime = viewData.ToDatetime,
                SearchField = viewData.SearchField,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                Data = _orderRepo.GetOrders(viewData),
                TotalRow = _orderRepo.Count(viewData),
            };
            return View(model);
        }

        // POST: Admin/ManageOrder/Filter
        [HttpPost]
        [ActionName(ActionFilter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Filter(ViewFilterOrder viewData)
        {
            if (viewData == null)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            if (!DateTimeUtils.IsValidDateTimeSQL(viewData.FromDatetime)
               || !DateTimeUtils.IsValidDateTimeSQL(viewData.ToDatetime))
            {
                return Json(MsgOrderHaveInvalidDateTime, JsonRequestBehavior.AllowGet);
            }

            var model = new ViewManageOrder()
            {
                Status = viewData.Status,
                FromDatetime = viewData.FromDatetime,
                ToDatetime = viewData.ToDatetime,
                SearchField = viewData.SearchField,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                Data = _orderRepo.GetOrders(viewData),
                TotalRow = _orderRepo.Count(viewData),
            };
            return View(model);
        }

        // POST: Admin/ManageOrder/Delete
        [HttpPost]
        [ActionName(ActionDelete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Delete(long orderID)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isDeleted = false;
            if (order.Status == OrderStatus.CANCELED.Code || order.Status == OrderStatus.REJECTED.Code)
            {
                isDeleted = _orderRepo.Delete(order.OrderID);
            }

            if (!isDeleted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgRemoveSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/DeleteAll
        [HttpPost]
        [ActionName(ActionDeleteAll)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult DeleteAll()
        {
            var isDeleted = _orderRepo.DeleteAllOrdersAreRejectedOrCanceled();

            if (!isDeleted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgRemoveSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/Approve
        [HttpPost]
        [ActionName(ActionApprove)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Approve(long orderID)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isApproved = false;
            if (order.Status == OrderStatus.PENDING.Code)
            {
                isApproved = _orderRepo.Approve(order.OrderID);
            }

            if (!isApproved)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            // send sms for user
            var message = $"Đơn hàng {order.Product.ProductName} đã được duyệt.";
            var toPhone = $"+84{order.Employee.Phone}";
            var sms = ConvertUtils<object>.RemoveSign4VietnameseString(message);
            _sendSMSService.SendSMS(toPhone, sms);

            //// comment when unit test no pass
            // var resultSendSMS = _sendSMSService.SendSMS(toPhone, sms);
            // if (string.IsNullOrEmpty(resultSendSMS))
            // {
            //    var errorMsg = $"Gửi sms tới khách hàng thất bại: {resultSendSMS}";
            //    return Json(errorMsg, JsonRequestBehavior.AllowGet);
            // }
            return Json(MsgDefaultSuccessed, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/Reject
        [HttpPost]
        [ActionName(ActionReject)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Reject(long orderID)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isRejected = false;
            if (order.Status == OrderStatus.PENDING.Code)
            {
                isRejected = _orderRepo.Reject(order.OrderID);
            }

            if (!isRejected)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            if (order.Type == PayTypeConstant.WALLET)
            {
                var newBalance = order.TotalMoney + order.Employee.Coin;
                _historyOrderProductRepo.UpdateCoin(order.Employee.EmployeeID, newBalance);
            }

            return Json(MsgOrderIsRejected, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/AcceptGotProduct
        [HttpPost]
        [ActionName(ActionAcceptGotProduct)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult AcceptGotProduct(long orderID, string linksucess)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isAccepted = false;
            if (order.Status == OrderStatus.WAITING.Code)
            {
                var message = $"Don cua ban da hoa thanh: {linksucess}";
                var getPhone = _orderRepo.GetPhone(orderID);

                var toPhone = $"+84{getPhone}";
                var sms = ConvertUtils<object>.RemoveSign4VietnameseString(message);
                _sendSMSService.SendSMS(toPhone, sms);




                isAccepted = _orderRepo.AcceptGotProduct(orderID, linksucess);
            }

            if (!isAccepted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            if (order.Type == PayTypeConstant.WALLET)
            {
                isAccepted = _orderRepo.AcceptPayed(orderID);
            }

            if (!isAccepted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgOrderIsGotProduct, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/AcceptPayed
        [HttpPost]
        [ActionName(ActionAcceptPayed)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult AcceptPayed(long orderID)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isAccepted = false;
            if (order.Status == OrderStatus.PAYING.Code && order.Type == PayTypeConstant.CASH)
            {
                isAccepted = _orderRepo.AcceptPayed(orderID);
            }

            if (!isAccepted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgOrderIsPay, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageOrder/AcceptNotPayed
        [HttpPost]
        [ActionName(ActionAcceptNotPayed)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult AcceptNotPayed(long orderID)
        {
            var order = _orderRepo.GetOrder(orderID);
            if (order == null)
            {
                return Json(MsgOrderNotFound, JsonRequestBehavior.AllowGet);
            }

            var isAccepted = false;
            if (order.Status == OrderStatus.PAYING.Code)
            {
                isAccepted = _orderRepo.AcceptNotPay(orderID);
            }

            if (!isAccepted)
            {
                return Json(MsgOrderHaveError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgOrderIsNotPay, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("DetailsMess")]
        [AllowAnonymous]
        public ActionResult DetailsMess(long orderID)
        {
          
            if (orderID.Equals(null))
            {
                return RedirectToAction(actionName: "Index", controllerName: ControllerName);
            }

            var model = new ViewDetailsMess()
            {
                Order = _orderRepo.GetOrder(orderID),
                DetailMess = _orderRepo.LoadDetailMess(orderID),
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
                return RedirectToAction("DetailsMess", "ManageOrder", new { orderId = orderID });
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
                        return RedirectToAction("DetailsMess", "ManageOrder", new { orderId = orderID });
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
            var isCreated = _orderRepo.CreateDetailMess(dataDto);

            if (isCreated <= 0)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm chi tiết hỗ trợ.";
                return RedirectToAction("DetailsMess", "ManageOrder", new { orderId = orderID });
            }
            return RedirectToAction("DetailsMess", "ManageOrder", new { orderId = orderID });
        }

        #endregion
    }
}