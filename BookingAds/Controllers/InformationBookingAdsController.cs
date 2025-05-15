using System;
using System.Web.Mvc;
using System.Web.Security;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Models.InformationBookingAds;
using BookingAds.Modules;
using BookingAds.Repository.InfomationProduct;
using BookingAds.Repository.InformationBookingAds;
using BookingAds.Repository.InformationBookingAds.Abstractions;
using BookingAds.Services.Payment;
using Newtonsoft.Json;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class InformationBookingAdsController : Controller
    {
        #region Repository
        private readonly IInformationBookingAdsRepository _informationRepo = new InformationBookingAdsRepository();
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        #endregion
        #region Service
        private readonly VnPayLibrary _vnpay = new VnPayLibrary();
        #endregion
        #region Constant
        private const int PAGE_SIZE = 6;
        private const string KeyErrorOrder = "Failed Order";
        private const string WALLET = nameof(WALLET);
        private const string CASH = nameof(CASH);
        private const string ControllerName = "InformationBookingAds";
        private const string ActionIndex = "Index";
        private const string ActionFilter = "Filter";
        private const string ActionOrderProduct = "OrderProduct";
        private const string ActionRecharge = "Recharge";
        private const string ActionReturnVnpay = "ReturnVnpay";
        private const string VnpVersion = "vnp_Version";
        private const string VnpCommand = "vnp_Command";
        private const string VnpTmnCode = "vnp_TmnCode";
        private const string VnpAmount = "vnp_Amount";
        private const string VnpBankCode = "vnp_BankCode";
        private const string VnpCreateDate = "vnp_CreateDate";
        private const string VnpCurrCode = "vnp_CurrCode";
        private const string VnpIpAddr = "vnp_IpAddr";
        private const string VnpLocale = "vnp_Locale";
        private const string VnpOrderInfo = "vnp_OrderInfo";
        private const string VnpOrderType = "vnp_OrderType";
        private const string VnpReturnUrl = "vnp_ReturnUrl";
        private const string VnpTxnRef = "vnp_TxnRef";
        private const string VnpPrefix = "vnp_";
        private const string VnpTransactionNo = "vnp_TransactionNo";
        private const string VnpResponseCode = "vnp_ResponseCode";
        private const string VnpTransactionStatus = "vnp_TransactionStatus";
        private const string VnpSecureHash = "vnp_SecureHash";
        private const string KeyMsgPaymentFailed = nameof(KeyMsgPaymentFailed);
        private const string KeyMsgPaymentSuccessed = nameof(KeyMsgPaymentSuccessed);
        private const string ValueMsgPaymentFailed = "Giao dịch thất bại";
        private const string ValueMsgPaymentFailedDatabase = "Có lỗi khi cập nhật số dư của khách hàng";
        private const long MinLevelMoney = 10000;
        private const long MaxLevelMoney = 50000000;
        private const string ValueMsgRechargeFailed = "Số tiền nạp vào không hợp lệ";
        #endregion
        #region History Order Product
        private const string ControllerHistoryOrderProduct = "HistoryOrderProduct";
        private const string ActionIndexHistoryOrderProduct = "Index";
        #endregion
        #region Action

        // GET: InformationBookingAds
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var viewData = new ViewFilterBookingAds()
            {
                SortField = SortField.Default,
                SortType = SortTypeConstant.DEFAULT,
                CatelogProductsID = 0,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
            };

            var model = new ViewIndex()
            {
                Data = _informationRepo.GetProducts(viewData),
                Sort = $"{viewData.SortField}-{viewData.SortType}",
                SortField = viewData.SortField,
                SortType = viewData.SortType,
                CatelogProductsID = viewData.CatelogProductsID,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _informationRepo.Count(viewData),
            };

            return View(model);
        }

        // POST: InformationBookingAds/Filter
        [HttpPost]
        [ActionName(ActionFilter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Filter(ViewFilterBookingAds viewData)
        {
            if (viewData == null)
            {
                return Json(ConstantsReturnMessengerOrder.ErrorNull, JsonRequestBehavior.AllowGet);
            }

            var model = new ViewIndex()
            {
                Data = _informationRepo.GetProducts(viewData),
                Sort = $"{viewData.SortField}-{viewData.SortType}",
                SortField = viewData.SortField,
                SortType = viewData.SortType,
                CatelogProductsID = viewData.CatelogProductsID,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _informationRepo.Count(viewData),
            };

            return View(model);
        }

        // POST: InformationBookingAds/OrderProduct
        [HttpPost]
        [ActionName(ActionOrderProduct)]
       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult OrderProduct(int employeeId, int productId, string payType)
        {
            IOrderProductRepository orderProductRepo = new OrderProductRepository();
            if (!ModelState.IsValid)
            {
                return RedirectToAction(ActionIndex, ControllerName);
            }

            if (orderProductRepo.CheckProductIsLocked(productId))
            {
                TempData.Add(KeyErrorOrder, ConstantsReturnMessengerOrder.ProductIsLocked);
                return Json(0, JsonRequestBehavior.AllowGet);

                // return RedirectToAction(ActionIndex, ControllerName);
            }

            int newOrderID = 0;
            if (WALLET.Equals(payType, StringComparison.OrdinalIgnoreCase))
            {
                if (!orderProductRepo.CheckCoin(employeeId, productId))
                {
                    TempData.Add(KeyErrorOrder, ConstantsReturnMessengerOrder.NotEnoughMoney);
                    return Json(0, JsonRequestBehavior.AllowGet);

                    // return RedirectToAction(ActionIndex, ControllerName);
                }
                else
                {
                    newOrderID = orderProductRepo.OrderProduct(employeeId, productId, PayTypeConstant.WALLET);
                    orderProductRepo.DeductionCoin(employeeId, productId);

                    // update employee coin
                    var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
                    var employee = _accountRepo.GetEmployee(currentEmployee.UserName);
                    var newCurrentEmployee = ConvertUtils<Employee>.Serialize(employee);
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);
                }
            }
            else if (CASH.Equals(payType, StringComparison.OrdinalIgnoreCase))
            {
                newOrderID = orderProductRepo.OrderProduct(employeeId, productId, PayTypeConstant.CASH);
            }

            var orderItem = orderProductRepo.GetOrder(newOrderID);
            return Json(orderItem, JsonRequestBehavior.AllowGet);

            // return RedirectToAction(actionName: ActionIndexHistoryOrderProduct, controllerName: ControllerHistoryOrderProduct);
        }

        [HttpGet]
        public ActionResult Details(long productId)
        {

            var dataview = new ViewDetailsProduct();

            dataview.Products = _informationRepo.LoadInfomationsProduct(productId);
            dataview.ProductAttributes = _informationRepo.GetProductAttributes(productId);

            dataview.ProductDescription = _informationRepo.GetProductDescription(productId);

            return View(dataview);
        }

        // GET: InformationBookingAds/Recharge
        [HttpGet]
        [ActionName(ActionRecharge)]
        public ActionResult Recharge()
        {
            var viewData = new ViewRecharge()
            {
               Money = 0,
            };
            return View(viewData);
        }

        // POST: InformationBookingAds/Recharge
        [HttpPost]
        [ActionName(ActionRecharge)]
        [ValidateAntiForgeryToken]
        public ActionResult Recharge(ViewRecharge dataDto)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            // check money
            if (dataDto.Money < MinLevelMoney || dataDto.Money > MaxLevelMoney)
            {
                TempData[KeyMsgPaymentFailed] = ValueMsgRechargeFailed;
                return View(dataDto);
            }

            // create url payment
            _vnpay.AddRequestData(VnpVersion, ConfigVnpayConstant.Version);
            _vnpay.AddRequestData(VnpCommand, ConfigVnpayConstant.Command);
            _vnpay.AddRequestData(VnpTmnCode, ConfigVnpayConstant.TmnCode);
            _vnpay.AddRequestData(VnpAmount, (dataDto.Money * 100).ToString());
            _vnpay.AddRequestData(VnpBankCode, ConfigVnpayConstant.BankCode);
            _vnpay.AddRequestData(VnpCreateDate, ConfigVnpayConstant.CreateDate);
            _vnpay.AddRequestData(VnpCurrCode, ConfigVnpayConstant.CurrCode);
            _vnpay.AddRequestData(VnpIpAddr, ConfigVnpayConstant.IpAddr);
            _vnpay.AddRequestData(VnpLocale, ConfigVnpayConstant.Locale);
            _vnpay.AddRequestData(VnpOrderInfo, dataDto.ContentPay.Trim());
            _vnpay.AddRequestData(VnpOrderType, ConfigVnpayConstant.OrderType);
            _vnpay.AddRequestData(VnpReturnUrl, ConfigVnpayConstant.ReturnUrl);
            string hello = Guid.NewGuid().ToString("N");
            _vnpay.AddRequestData(VnpTxnRef, hello);

            var vnUrl = new Uri(ConfigVnpayConstant.Url);
            var paymentUrl = _vnpay.CreateRequestUrl(vnUrl, ConfigVnpayConstant.HashSecret);
            return Redirect(paymentUrl);
        }

        // GET: InformationBookingAds/ReturnVnpay
        [HttpGet]
        [ActionName(ActionReturnVnpay)]
        public ActionResult ReturnVnpay()
        {
            var vnpayData = Request.QueryString;
            foreach (string s in vnpayData)
            {
                // get all querystring data
                if (!string.IsNullOrEmpty(s) && s.StartsWith(VnpPrefix, StringComparison.CurrentCulture))
                {
                    _vnpay.AddResponseData(s, vnpayData[s]);
                }
            }

            // get response
            var tranId = Convert.ToInt64(_vnpay.GetResponseData(VnpTransactionNo));
            var responseCode = _vnpay.GetResponseData(VnpResponseCode);
            var transactionStatus = _vnpay.GetResponseData(VnpTransactionStatus);
            var secureHash = Request.QueryString[VnpSecureHash];
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            var money = Convert.ToInt64(_vnpay.GetResponseData(VnpAmount)) / 100;

            var checkSignature = _vnpay.ValidateSignature(secureHash, ConfigVnpayConstant.HashSecret);
            if (checkSignature)
            {
                if (responseCode == ConfigVnpayConstant.Successed && transactionStatus == ConfigVnpayConstant.Successed)
                {
                    var response = new
                    {
                        TransactionID = tranId,
                        ResponseCode = responseCode,
                        TransactionStatus = transactionStatus,
                    };

                    // add money in database
                    var isUpdated = _informationRepo.Recharge(currentEmployee.UserName, money);
                    if (!isUpdated)
                    {
                        TempData[KeyMsgPaymentFailed] = ValueMsgPaymentFailedDatabase;
                        return RedirectToAction(ActionRecharge, ControllerName);
                    }

                    TempData[KeyMsgPaymentSuccessed] = $"Giao dịch thành công";
                    var employee = _accountRepo.GetEmployee(currentEmployee.UserName);
                    var newCurrentEmployee = ConvertUtils<Employee>.Serialize(employee);
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);
                    return RedirectToAction(ActionIndex, ControllerName);
                }
            }

            TempData[KeyMsgPaymentFailed] = ValueMsgPaymentFailed;

            return RedirectToAction(ActionRecharge, ControllerName);
        }

        #endregion
    }
}