using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Models.Account;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class AccountController : Controller
    {
        #region Repository
        private readonly IAccountRepository _accountRepo = new AccountRepository();
        #endregion
        #region Constant
        private const string STORAGE_ROOT_IMAGES_ADMIN = "~/Images/Admin";
        private const string ControllerName = "Account";
        private const string ActionLogin = "Login";
        private const string ActionChangePassword = "ChangePassword";
        private const string ActionLogout = "Logout";
        private const string ActionUpdate = "Update";
        private const string KeyErrorLogin = "Failed Login";
        private const string ValueErrorLogin = "Tài khoản hoặc mật khẩu không chính xác";
        private const string KeyErrorChangePassword = "Failed Change Password";
        private const string ValueErrorChangePassword = "Thay đổi mật khẩu thất bại";
        private const string ValueSuccessChangePassword = "Thay đổi mật khẩu thành công";
        private const string MsgAvatarUploadedIsRequired = "Yêu cầu tải Ảnh đại diện";
        private const string MsgUploadFileFailed = "Cập nhật ảnh đại diện của quản trị viên thất bại";
        private const string MsgUploadFileExtensionError = "Tệp tải lên phải thuộc các loại sau: .png, .jpg, .gif, .jpeg, .tiff";
        private const string MsgUploadFileSuccessed = "Thay đổi Ảnh đại diện của quản trị viên thành công";
        #endregion
        #region Dashboard
        private const string ControllerDashboard = "Dashboard";
        private const string ActionIndexDashboard = "Index";
        #endregion
        #region Manage Employee
        private const string ControllerManageEmployee = "ManageEmployee";
        private const string ActionIndexManageEmployee = "Index";
        #endregion
        #region Action

        // GET: Admin/Account/Login
        [AllowAnonymous]

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [HttpGet]
        [ActionName(ActionLogin)]
        public ActionResult Login()
        {
            var currentAdmin = ConvertUtils<Entities.Admin>.Deserialize(User.Identity.Name);
            if (currentAdmin != null)
            {
                return RedirectToAction(ActionIndexDashboard, ControllerDashboard);
            }

            return View();
        }

        // POST: Admin/Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [HttpPost]
        [ActionName(ActionLogin)]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ViewLogin dataDto)
        {
            // validate data dto
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            var currentAdminIsExisted = ConvertUtils<Entities.Admin>.Deserialize(User.Identity.Name);
            if (currentAdminIsExisted != null)
            {
                return RedirectToAction(ActionIndexDashboard, ControllerDashboard);
            }

            var admin = _accountRepo.Login(dataDto) as Entities.Admin;
            if (admin == null)
            {
                ModelState.AddModelError(KeyErrorLogin, ValueErrorLogin);
                return View(dataDto);
            }

            // set data and status login in cookie
            var currentAdmin = ConvertUtils<Entities.Admin>.Serialize(admin);
            FormsAuthentication.SetAuthCookie(currentAdmin, false);

            return RedirectToAction(ActionIndexDashboard, ControllerDashboard);
        }

        // GET: Admin/Account/ChangePassword
        [HttpGet]
        [ActionName(ActionChangePassword)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Admin/Account/ChangePassword
        [HttpPost]
        [ActionName(ActionChangePassword)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ViewChangePassword dataDto)
        {
            if (!ModelState.IsValid)
            {
                return View(dataDto);
            }

            var currentUser = ConvertUtils<Entities.Account>.Deserialize(User.Identity.Name);
            dataDto.CurrentUserName = currentUser.UserName;
            var isChanged = _accountRepo.UpdateNewPassword(dataDto);

            if (!isChanged)
            {
                ModelState.AddModelError(KeyErrorChangePassword, ValueErrorChangePassword);
                return View(dataDto);
            }

            ViewBag.Success = ValueSuccessChangePassword;

            return View();
        }

        // GET: Admin/Account/Logout
        [HttpGet]
        [ActionName(ActionLogout)]
        public ActionResult Logout()
        {
            // remove status login
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction(ActionLogin);
        }

        // POST: Admin/Account/Update
        [HttpPost]
        [ActionName(ActionUpdate)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Update(HttpPostedFileBase avatarUploaded)
        {
            var currentAdmin = ConvertUtils<Entities.Account>.Deserialize(User.Identity.Name);
            if (currentAdmin == null
                || avatarUploaded == null
                || (avatarUploaded != null && avatarUploaded.ContentLength == 0))
            {
                return Json(MsgAvatarUploadedIsRequired, JsonRequestBehavior.AllowGet);
            }

            // config upload
            var rootPath = Server.MapPath(STORAGE_ROOT_IMAGES_ADMIN);

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

            var isUpdated = _accountRepo.UpdateAvatarOfAdmin(currentAdmin.UserName, uniqueFileName);
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
            var newAdmin = _accountRepo.GetAdmin(currentAdmin.UserName);
            var newCurrentAdmin = ConvertUtils<Entities.Admin>.Serialize(newAdmin);
            FormsAuthentication.SetAuthCookie(newCurrentAdmin, false);

            return Json(MsgUploadFileSuccessed, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}