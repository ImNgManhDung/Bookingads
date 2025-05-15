using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.ManageEmployee;
using BookingAds.Areas.Admin.Repository.Employee;
using BookingAds.Areas.Admin.Repository.Employee.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Models.Message;
using BookingAds.Common.Repository.Employee;
using BookingAds.Common.Repository.Message.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Modules;
using OfficeOpenXml;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class ManageEmployeeController : Controller
    {
        #region Repository
        private readonly IEmployeeRepository _employeeRepo = new EmployeeRepository();
        private readonly IMessageRepository _messageRepo = new MessageRepository();
        #endregion
        #region Constant
        private const int PAGE_SIZE = 4;
        private const string ControllerName = "ManageEmployee";
        private const string ActionIndex = "Index";
        private const string ActionFilter = "Filter";
        private const string ActionLock = "Lock";
        private const string ActionUnlock = "Unlock";
        private const string ActionReport = "Report";
        private const string ActionChat = "Chat";
        private const string ActionChatHistory = "ChatHistory";
        private const string ActionRead = "Read";
        private const string ActionEdit = "Edit";
        private const string ActionDelete = "Delete";
        private const string MsgHasError = "Có lỗi";
        private const string MsgEmployeeIsNotExists = "Khách hàng không tồn tại";
        private const string MsgFail = "Thất bại";
        private const string MsgSuccess = "Thành công";
        private const string MsgCurrentUserIsNotSender = "Người dùng hiện tại không phải là người gửi";
        #endregion
        #region Action

        // GET: Admin/ManageEmployee
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var viewData = new ViewFilterEmployee()
            {
                Gender = FilterGender.Default,
                Field = FilterField.FullName,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
            };
            var model = new ViewManageEmployee()
            {
                Gender = viewData.Gender,
                Field = viewData.Field,
                SearchValue = viewData.SearchValue,
                Data = _employeeRepo.GetEmployees(viewData),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _employeeRepo.Count(viewData),
            };
            return View(model);
        }

        // POST:  Admin/ManageEmployee/Filter
        [HttpPost]
        [ActionName(ActionFilter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Filter(ViewFilterEmployee viewData)
        {
            if (viewData == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var model = new ViewManageEmployee()
            {
                Gender = viewData.Gender,
                Field = viewData.Field,
                SearchValue = viewData.SearchValue,
                Data = _employeeRepo.GetEmployees(viewData),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _employeeRepo.Count(viewData),
            };
            return View(model);
        }

        // POST: Admin/ManageEmployee/Lock
        [HttpPost]
        [ActionName(ActionLock)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Lock(long employeeID = 0)
        {
            if (employeeID == 0)
            {
                return Json(MsgEmployeeIsNotExists, JsonRequestBehavior.AllowGet);
            }

            var isLocked = _employeeRepo.UpdateStatusAccount(AccountStatusConstant.LOCK, employeeID);
            if (!isLocked)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageEmployee/Unlock
        [HttpPost]
        [ActionName(ActionUnlock)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Unlock(long employeeID = 0)
        {
            if (employeeID == 0)
            {
                return Json(MsgEmployeeIsNotExists, JsonRequestBehavior.AllowGet);
            }

            var isLocked = _employeeRepo.UpdateStatusAccount(AccountStatusConstant.UNLOCK, employeeID);
            if (!isLocked)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/ManageEmployee/Report
        [HttpGet]
        [ActionName(ActionReport)]
        public void Report()
        {
            var viewData = new ViewFilterEmployee()
            {
                Gender = FilterGender.Default,
                Field = FilterField.FullName,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = 0,
            };

            var employees = _employeeRepo.GetEmployees(viewData);

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet sheet = excelPackage.Workbook.Worksheets.Add("Reports");
                sheet.Cells["A1"].Value = "Họ và tên";
                sheet.Cells["B1"].Value = "Tên đăng nhập";
                sheet.Cells["C1"].Value = "Giới tính";
                sheet.Cells["D1"].Value = "Số dư tài khoản";
                int row = 2;
                foreach (var item in employees)
                {
                    sheet.Cells[string.Format("A{0}", row)].Value = $"{item.LastName} {item.FirstName}";
                    sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                    sheet.Cells[string.Format("C{0}", row)].Value = item.Gender ? "Nam" : "Nữ";
                    sheet.Cells[string.Format("D{0}", row)].Value = item.Coin.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("vi-VN"));
                    row++;
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment: filename = {DateTime.Now.Ticks}_Report_Employee.xlsx");
                Response.BinaryWrite(excelPackage.GetAsByteArray());
            }

            Response.End();
        }

        // GET: Admin/ManageEmployee/Chat
        [HttpGet]
        [ActionName(ActionChat)]
        public ActionResult Chat()
        {
            var model = new ViewChat()
            {
                Employees = _messageRepo.GetEmployees(string.Empty),
            };

            return View(model);
        }

        // GET: Admin/ManageEmployee/ChatHistory
        [HttpGet]
        [ActionName(ActionChatHistory)]
        public ActionResult ChatHistory(string userName = "", int limit = 5)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

            var myUserName = ConvertUtils<Account>.Deserialize(User.Identity.Name).UserName;
            var model = new ViewChatHistory()
            {
                Messages = _messageRepo.GetMessages(myUserName, userName, limit),
            };

            return View(model);
        }

        // POST: Admin/ManageEmployee/Chat
        [HttpPost]
        [ActionName(ActionChat)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Chat(ViewCreateMessage dataDto)
        {
            if (dataDto == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var newMessageID = _messageRepo.CreateMessage(dataDto);
            if (newMessageID == 0)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(newMessageID, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageEmployee/Read
        [HttpPost]
        [ActionName(ActionRead)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Read(ViewReadMessage dataDto)
        {
            if (dataDto == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var isRead = _messageRepo.ReadMessage(dataDto);
            if (!isRead)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageEmployee/Edit
        [HttpPost]
        [ActionName(ActionEdit)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Edit(ViewEditMessage dataDto)
        {
            if (dataDto == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var currentAdmin = ConvertUtils<Account>.Deserialize(User.Identity.Name);

            if (dataDto.SenderID != currentAdmin.UserName)
            {
                return Json(MsgCurrentUserIsNotSender, JsonRequestBehavior.AllowGet);
            }

            var isEdited = _messageRepo.EditMessage(dataDto);
            if (!isEdited)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageEmployee/Delete
        [HttpPost]
        [ActionName(ActionDelete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Delete(long messageID = 0, string senderID = "")
        {
            if (messageID == 0 || string.IsNullOrEmpty(senderID))
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var currentAdmin = ConvertUtils<Account>.Deserialize(User.Identity.Name);

            if (senderID != currentAdmin.UserName)
            {
                return Json(MsgCurrentUserIsNotSender, JsonRequestBehavior.AllowGet);
            }

            var isDeleted = _messageRepo.DeleteMessage(messageID, senderID);
            if (!isDeleted)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}