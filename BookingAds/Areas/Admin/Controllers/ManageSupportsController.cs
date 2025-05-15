using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.Supports;
using BookingAds.Areas.Admin.Repository.Supports;
using BookingAds.Areas.Admin.Repository.Supports.Abstractions;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Controllers
{
    public class ManageSupportsController : Controller
    {
        #region Repository

        private readonly ISupportsRepository _supportsRepository = new SupportsRepository();
        private readonly IAccountRepository _acountRepository = new AccountRepository();
        private readonly string KeyErrorOderID = "error_OderID";
        private readonly string ValueNotOderID = "Bắt buộc nhập dịch vụ";
        private readonly string ValueIsNull = "-99";


        #endregion
        #region Constant
        private const int PAGE_SIZE = 10;
        private const string FormatDateTimeFilter = "yyyy-MM-ddTHH:mm";
        private const string ControllerName = "SupportController";
        private const string ActionIndex = "Index";
        private const string ActionDetails = "Details";
        private const string ActionSearchRequest = "SearchRequest";
        private const string ActionProducts = "Products";
        private const string ActionEdit = "Edit";
        private const string ActionCanceled = "Canceled";
        private const string STORAGE_ROOT_IMAGES_Detail = "/Images/SuportDetails/";
        private const string STORAGE_ROOT_IMAGES_SUPPORT = "/Images/Support/";
        private const string KeyErrorMessage = "error_message";
        private const string ValueNotMessage = "Nội dung bắt buộc phải có";
        #endregion

        // GET: Admin/ManageSupports
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            ViewBag.Title = "Yêu Cầu";

            var viewData = new ViewFilterSupports()
            {
                OrderStatus = OrderStatus.DEFAULT,
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
                DateStart = string.Empty,
                DateEnd = string.Empty,
            };
            var totalRow = _supportsRepository.Count(viewData);
            var model = new ViewIndex()
            {
                DateStart = viewData.DateStart,
                DateEnd = viewData.DateEnd,
                OrderStatus = viewData.OrderStatus,
                SearchValue = viewData.SearchValue,
                Data = _supportsRepository.LoadSupport(viewData),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = totalRow,
            };

            return View(model);
        }

        // Post: Admin/ManageSupports/SearchSearchRequest
        [HttpPost]
        [ActionName(ActionSearchRequest)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult SearchRequest(ViewFilterSupports viewData)
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

            var model = new ViewIndex()
            {
                DateEnd = viewData.DateEnd,
                DateStart = viewData.DateStart,
                OrderStatus = viewData.OrderStatus,
                SearchValue = viewData.SearchValue,
                Data = _supportsRepository.LoadSupport(viewData),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _supportsRepository.Count(viewData),
            };

            return View(model);
        }

        // GET: Admin/ManageSupports/Details/5
        [HttpGet]
        [ActionName("Details")]
        [AllowAnonymous]
        public ActionResult Details(long supportsID)
        {
            if (supportsID.Equals(null))
            {
                return RedirectToAction(actionName: "Index", controllerName: "ManageSupports");
            }

            var model = new ViewDetails()
            {
                Supports = _supportsRepository.GetSupports(supportsID),
                SupportsDetail = _supportsRepository.LoadSupportDetail(supportsID),
            };

            //if (model.SupportsDetail.Count == 0)
            //{
            //    return RedirectToAction(actionName: "Index", controllerName: "Support");
            //}
            //return Json(model, JsonRequestBehavior.AllowGet);
            return View(model);
        }
        [HttpPost]
        [ActionName("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ViewDetails dataDto, HttpPostedFileBase photoUploaded)
        {
            //validate
            if (!ModelState.IsValid)
            {
                return View("Details", dataDto);
            }

            if (dataDto.Messenger == null)
            {
                TempData[KeyErrorMessage] = ValueNotMessage;
                //  return RedirectToAction(ActionDetails, "Support", new { SupportsID = isCreated });
                return RedirectToAction(ActionDetails, "ManageSupports", new { SupportsID = dataDto.Supports.SupportsID });
            }

            // config upload
            string rootPath = Server.MapPath(STORAGE_ROOT_IMAGES_Detail);
            string path = null;

            // check upload when add product
            if (photoUploaded == null)
            {
                //TempData[KeyErrorMessage] = ValueNotUploadFileWhenAdd;
                return View("Add", dataDto);
            }

            if (photoUploaded != null && photoUploaded.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(photoUploaded.FileName).ToLower();

                if (!UploadFileConstant.AllowedExtensions.Contains(fileExtension)
                    || UploadFileConstant.RejectExtensions.Contains(fileExtension))
                {
                    //TempData[KeyErrorMessage] = ValueErrorUploadFile;
                    return View("Details", dataDto);
                }

                var uniqueFileName = $"{DateTime.Now.Ticks}_{photoUploaded.FileName}";

                dataDto.Images = uniqueFileName;

                // create storage path
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                path = Path.Combine(rootPath, Path.GetFileName(dataDto.Images));
            }

            // create new product
            //if (dataDto.Supports.SupportsID == 0)
            //{
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);


            var isCreated = _supportsRepository.CreateSupportsDetail(dataDto);
            if (isCreated <= 0)
            {
                //TempData[KeyErrorMessage] = ValueErrorAddProduct;
                return View(ActionDetails, dataDto);
            }

            // }

            // save file uploaed
            if (path != null)
            {
                photoUploaded.SaveAs(path);
            }

            return RedirectToAction(ActionDetails, "ManageSupports", new { SupportsID = dataDto.Supports.SupportsID });
        }
    }
}