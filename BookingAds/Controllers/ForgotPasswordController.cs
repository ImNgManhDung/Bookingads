using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Attributes.Filters;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Models.ForgotPassword;
using BookingAds.Modules;
using BookingAds.Repository.ForgotPassword;
using BookingAds.Repository.ForgotPassword.Abstractions;
using BookingAds.Services;
using BookingAds.Services.Abstractions;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class ForgotPasswordController : Controller
    {
        #region Reprository
        private readonly IForgotPasswordRepository _forgotPasswordRepository = new ForgotPasswordRepository();
        #endregion
        #region Service
        private readonly ISendSMSService _sendSMSService = new TwilioSendSMSApiService();
        #endregion
        #region Constant
        private const string Forgot = "Tiếp tục";
        private const string Confirm = "Xác Nhận";
        private const string Fail = "fail";
        private const string FailToken = "failToken";
        private const string Success = "success";
        private const string NextPage = "next";
        private const string IsPass = "isPass";
        private const string ControllerInformationBookingAds = "InformationBookingAds";
        private const string ActionIndexInformationBookingAds = "Index";
        #endregion

        #region Action

        // GET: ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Index")]
        public ActionResult Index(string result)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            var viewdata = new ViewForgotPassword();
            ViewBag.Title = "Tìm Tài Khoản";

            if (Fail.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsFail = true;
            }

            if (FailToken.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsFailToken = true;
            }

            if (Success.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsSuccess = true;
            }

            return View(viewdata);
        }

        // POST: ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ViewForgotPassword viewdata, string result, string buttonAction)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            ViewBag.Title = "Tìm Tài Khoản";

            viewdata = viewdata ?? new ViewForgotPassword();

            if (!ModelState.IsValid)
            {
                return View(viewdata);
            }

            // validate
            if (Forgot.Equals(buttonAction, StringComparison.OrdinalIgnoreCase))
            {
                const string emailKey = nameof(ViewForgotPassword.Username);
                ModelUtils.RemoveError(ModelState, emailKey);

                if (ModelState.IsValidField(emailKey))
                {
                    string email = viewdata.Username;

                    var checkUserName = _forgotPasswordRepository.CheckUserName(email);

                    if (!checkUserName)
                    {
                        return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword", routeValues: new { result = Fail });
                    }
                    else
                    {
                        var getPhone = _forgotPasswordRepository.GetPhone(email);
                        if (getPhone == null)
                        {
                            return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword", routeValues: new { result = Fail });
                        }

                        string token = GenTokenUtils.GenerateToken(20);

                        // Use string when Send mail for user
                        string genToken = _forgotPasswordRepository.CreateToken(token, viewdata.Username);
                        string authencode = _forgotPasswordRepository.GetCode(viewdata);

                        var message = $"Mã xác thực là: {authencode}";
                        // SendMail(viewdata.Username, token: genToken, authenCode: authencode);
                        // int[] numbers = ExtralNumberUtils.ConvertStringToNumbers(authencode);
                        // string[] words = ExtralNumberUtils.ConvertNumbersToWords(numbers);
                        // string msg = ExtralNumberUtils.ConvertArrayToString(message);
                        var toPhone = $"+84{getPhone}";
                        var sms = ConvertUtils<object>.RemoveSign4VietnameseString(message);
                        _sendSMSService.SendSMS(toPhone, sms);
                        return RedirectToAction(actionName: "ConfirmCode", controllerName: "ForgotPassword", routeValues: new { username = $"{email}", result = NextPage });
                    }
                }
                else
                {
                    return View(viewdata);
                }
            }

            return View(viewdata);
        }

        // GET: ForgotPassword/ConfirmCode
        [HttpGet]
        [AllowAnonymous]
        [ActionName("ConfirmCode")]
        public ActionResult ConfirmCode(string result, string username)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            if (Request.UrlReferrer == null || !"fail,success,next".Split(',').Any(o => o.Equals(result, StringComparison.OrdinalIgnoreCase)))
            {
                return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword");
            }

            var viewdata = new ViewConfirmCode();

            if (Fail.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsFail = true;
            }
            else if (Success.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsSuccess = true;
            }
            
            ViewBag.Title = "Xác nhận mã bảo mật";
            return View(viewdata);
        }

        // POST: ForgotPassword/ConfirmCode
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ConfirmCode")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCode(ViewConfirmCode viewdata, string result, string buttonAction)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            viewdata = viewdata ?? new ViewConfirmCode();
            ViewBag.Title = "Xác nhận mã bảo mật";

            if (Confirm.Equals(buttonAction, StringComparison.OrdinalIgnoreCase))
            {
                if (!ModelState.IsValid)
                {
                    return View(viewdata);
                }

                const string confirmCode = nameof(ViewConfirmCode.ConfirmCode);
                ModelUtils.RemoveError(ModelState, confirmCode);

                if (ModelState.IsValidField(confirmCode))
                {
                    var checkConfirmCode = _forgotPasswordRepository.CheckConfirmCode(viewdata);

                    if (!checkConfirmCode)
                    {
                        return RedirectToAction(actionName: "ConfirmCode", controllerName: "ForgotPassword", routeValues: new { result = Fail, username = $"{viewdata.Username}" });
                    }
                    else
                    {
                        // Get Token account
                        string genToken = _forgotPasswordRepository.GetToken(viewdata.Username);
                        return RedirectToAction(actionName: "ChangePassword", controllerName: "ForgotPassword", routeValues: new { result = NextPage, username = $"{viewdata.Username}", token = $"{genToken}" });
                    }
                }
            }

            return View(viewdata);
        }

        // GET: ForgotPassword/ChangePassword
        [HttpGet]
        [AllowAnonymous]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword(string result, string username, string token)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            if (Request.UrlReferrer == null || !"fail,success,next,isPass".Split(',').Any(o => o.Equals(result, StringComparison.OrdinalIgnoreCase)))
            {
                return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword");
            }

            if (Fail.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsFail = true;
            }
            else if (IsPass.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsPass = true;
            }
            else if (Success.Equals(result, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsSuccess = true;
            }

            bool checktoken = _forgotPasswordRepository.CheckToken(token, username);

            if (!checktoken)
            {
                return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword");
            }

            ViewBag.Title = "Đôi mật khẩu mới";
            var viewdata = new ViewChangePassword();
            return View(viewdata);
        }

        // POST: ForgotPassword/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ChangePassword")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ViewChangePassword viewdata, string result)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            viewdata = viewdata ?? new ViewChangePassword();

            if (!ModelState.IsValid)
            {
                return View(viewdata);
            }

            ViewBag.Title = "Đôi mật khẩu mới";

            const string password = nameof(ViewChangePassword.Password);
            ModelUtils.RemoveError(ModelState, password);

            string ispass = _forgotPasswordRepository.IsPassWord(viewdata.Username);

            if (!string.IsNullOrEmpty(ispass) && ispass == PasswordUtils.MD5(viewdata.Password))
            {
                return RedirectToAction(actionName: "ChangePassword", controllerName: "ForgotPassword", routeValues: new { result = IsPass, username = $"{viewdata.Username}", token = $"{viewdata.Token}" });
            }

            bool checkTimeToken = _forgotPasswordRepository.CheckTimeToken(viewdata.Token, viewdata.Username);

            if (checkTimeToken)
            {
                // Delete Token when access time out
                _forgotPasswordRepository.DeleteToken(viewdata.Token, viewdata.Username);
                return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword", routeValues: new { result = FailToken, });
            }
            else
            {
                bool changePass = _forgotPasswordRepository.ChangePassWord(viewdata);
                if (!changePass)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword", routeValues: new { result = Fail, });
                }
                else
                {
                    // Delete Token when successfully changing password
                    _forgotPasswordRepository.DeleteToken(viewdata.Token, viewdata.Username);
                    return RedirectToAction(actionName: "Index", controllerName: "ForgotPassword", routeValues: new { result = Success, });
                }
            }
        }
        #endregion

        #region Non-Action
        private static void SendMail(string memberCode, string token, string authenCode)
        {
            var request = System.Web.HttpContext.Current.Request;

            var urlHost = request.Url.GetLeftPart(System.UriPartial.Authority);

            string callbackUrl = $"{urlHost}/ForgotPassword/ChangePassword?result=success&username={memberCode}&Token={token}";

            // Create Uri from Url
            Uri uriCallbackUrl = new Uri(callbackUrl);
            string email = memberCode + "fpt.com";

            string emailto = "nmdung181002@gmail.com";
            SendMailUtils.SendMail(emailto, uriCallbackUrl, authenCode);
        }

        #endregion
    }
}