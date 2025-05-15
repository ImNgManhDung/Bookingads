using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using BookingAds.Areas.Admin.Models.ManageEmployee;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Models.Account;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Modules;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class AccountController : Controller
    {
        #region Repository
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        #endregion
        #region Constant
        private const string STORAGE_ROOT_IMAGES_EMPLOYEE = "~/Images/Employees";
        private const string ControllerName = "Account";
        private const string ActionLogin = "Login";
        private const string ActionHome = "Home";
        private const string ActionChangePassword = "ChangePassword";
        private const string ActionLogout = "Logout";
        private const string ActionUpdate = "Update";
        private const string ActionUpdateAvatar = "UpdateAvatar";
        private const string KeyErrorLogin = "Failed Login";
        private const string KeyErrorChangePassword = "Failed Change Password";
        private const string ValueChangePasswordSuccess = "Thay đổi mật khẩu thành công";
        private const string KeyErrorUpdateEmployeeInfo = "Error Update Employee Info";
        private const string ValueEmployeeIsNotExisted = "Khách hàng không tồn tại";
        private const string ValueUpdateEmployeeInfoFailed = "Cập nhật thông tin cá nhân của khách hàng thất bại";
        private const string MsgUpdateEmployeeInfoSuccessed = "Cập nhật thông tin cá nhân của khách hàng thành công";
        private const string MsgAvatarUploadedIsRequired = "Yêu cầu tải Ảnh đại diện";
        private const string MsgUploadFileFailed = "Cập nhật ảnh đại diện thất bại";
        private const string MsgUploadFileExtensionError = "Tệp tải lên phải thuộc các loại sau: .png, .jpg, .gif, .jpeg, .tiff";
        private const string MsgUploadFileSuccessed = "Thay đổi Ảnh đại diện thành công";

        #endregion
        #region Information Product
        private const string ControllerInformationBookingAds = "InformationBookingAds";
        private const string ActionIndexInformationBookingAds = "Index";
        #endregion
        #region Action

        // GET: Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [HttpGet]
        [ActionName(ActionLogin)]
        public ActionResult Login(Uri returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee != null)
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }

            return View();
        }

        // POST: Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [HttpPost]
        [ActionName(ActionLogin)]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ViewLogin dataDto, Uri returnUrl)
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

            var isLocked = _accountRepo.IsLockedEmployee(dataDto.UserName);
            if (isLocked)
            {
                ModelState.AddModelError(KeyErrorLogin, ConstantsReturnMessengerAccount.ErrorEmployeeIsLocked);
                return View(dataDto);
            }

            var employee = _accountRepo.Login(dataDto) as Employee;
            if (employee == null)
            {
                ModelState.AddModelError(KeyErrorLogin, ConstantsReturnMessengerAccount.ErrorLogin);
                return View(dataDto);
            }

            // set data for cookie
            var currentEmployee = ConvertUtils<Employee>.Serialize(employee);
            FormsAuthentication.SetAuthCookie(currentEmployee, false);

            // Kiểm tra ReturnUrl
            if (returnUrl != null && Url.IsLocalUrl(returnUrl.ToString()))
            {
                return Redirect(returnUrl.ToString());
            }
            else
            {
                return RedirectToAction(ActionIndexInformationBookingAds, ControllerInformationBookingAds);
            }
        }

        // GET: Account/ChangePassword
        [HttpGet]
        [ActionName(ActionChangePassword)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ActionName(ActionChangePassword)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ViewChangePassword dataDto)
        {
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            var currentUser = ConvertUtils<Account>.Deserialize(User.Identity.Name);
            dataDto.CurrentUserName = currentUser.UserName;
            var isChanged = _accountRepo.UpdateNewPassword(dataDto);

            if (!isChanged)
            {
                ModelState.AddModelError(KeyErrorChangePassword, ConstantsReturnMessengerAccount.ErrorChangePassword);
                return View(dataDto);
            }

            ViewBag.Success = ValueChangePasswordSuccess;

            return View();
        }

        // GET: Account/Logout
        [HttpGet]
        [ActionName(ActionLogout)]
        public ActionResult Logout()
        {
            // clean login
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Update
        [HttpGet]
        [ActionName(ActionUpdate)]
        public ActionResult Update()
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            var viewData = new ViewUpdateEmployeeInfo()
            {
                UserName = currentEmployee.UserName,
                FirstName = currentEmployee.FirstName,
                LastName = currentEmployee.LastName,
                Phone = currentEmployee.Phone,
                Gender = currentEmployee.Gender ? FilterGender.Man : FilterGender.Woman,
                Coin = currentEmployee.Coin,
                Avatar = currentEmployee.Avatar,
            };

            return View(viewData);
        }

        // POST: Account/Update
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [HttpPost]
        [ActionName(ActionUpdate)]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ViewUpdateEmployeeInfo dataDto)
        {
            var employeeExisted = _accountRepo.GetEmployee(dataDto.UserName);
            if (employeeExisted == null)
            {
                ModelState.AddModelError(KeyErrorUpdateEmployeeInfo, ValueEmployeeIsNotExisted);
                return View(dataDto);
            }

            dataDto.Avatar = employeeExisted.Avatar;
            dataDto.Coin = employeeExisted.Coin;

            // validate
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            var isUpdated = _accountRepo.UpdateEmployee(dataDto);
            if (!isUpdated)
            {
                ModelState.AddModelError(KeyErrorUpdateEmployeeInfo, ValueUpdateEmployeeInfoFailed);
                return View(dataDto);
            }

            // set form auth
            var newEmployee = _accountRepo.GetEmployee(dataDto.UserName);
            var newCurrentEmployee = ConvertUtils<Employee>.Serialize(newEmployee);
            FormsAuthentication.SignOut();
            FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);

            ViewBag.Success = MsgUpdateEmployeeInfoSuccessed;

            return View(dataDto);
        }

        // POST: Account/UpdateAvatar
        [HttpPost]
        [ActionName(ActionUpdateAvatar)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult UpdateAvatar(HttpPostedFileBase avatarUploaded)
        {
            var currentEmployee = ConvertUtils<Account>.Deserialize(User.Identity.Name);
            if (currentEmployee == null
                || avatarUploaded == null
                || (avatarUploaded != null && avatarUploaded.ContentLength == 0))
            {
                return Json(MsgAvatarUploadedIsRequired, JsonRequestBehavior.AllowGet);
            }

            // config upload
            var rootPath = Server.MapPath(STORAGE_ROOT_IMAGES_EMPLOYEE);

            var fileExtension = Path.GetExtension(avatarUploaded.FileName).ToLower();

            if (!UploadFileConstant.AllowedExtensions.Contains(fileExtension)
                || UploadFileConstant.RejectExtensions.Contains(fileExtension))
            {
                return Json(MsgUploadFileExtensionError, JsonRequestBehavior.AllowGet);
            }

            var uniqueFileName = $"{DateTime.Now.Ticks}_{avatarUploaded.FileName}";

            // create storage path
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var path = Path.Combine(rootPath, Path.GetFileName(uniqueFileName));

            var isUpdated = _accountRepo.UploadAvatarOfEmployee(currentEmployee.UserName, uniqueFileName);
            if (!isUpdated)
            {
                return Json(MsgUploadFileFailed, JsonRequestBehavior.AllowGet);
            }

            // save file uploaed
            if (path != null)
            {
                avatarUploaded.SaveAs(path);
            }

            // set form auth
            var newEmployee = _accountRepo.GetEmployee(currentEmployee.UserName);
            var newCurrentEmployee = ConvertUtils<Employee>.Serialize(newEmployee);
            FormsAuthentication.SetAuthCookie(newCurrentEmployee, false);

            return Json(MsgUploadFileSuccessed, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}