using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.ManageProduct;
using BookingAds.Areas.Admin.Repository.Product;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Common.Repository.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Models.Supports;
using BookingAds.Modules;
using BookingAds.Repository.Supports;
using BookingAds.Repository.Supports.Abstractions;

namespace BookingAds.Controllers
{
    [RoleFilter(Roles = RoleConstant.EMPLOYEE)]
    public class SupportController : Controller
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

        // GET: Support
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Yêu Cầu";
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee == null )
            {
                return RedirectToAction("Login", "Account");
            }

            var viewData = new ViewFilterSupports()
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
                Data = _supportsRepository.LoadSupport(viewData, currentEmployee.EmployeeID),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _supportsRepository.Count(viewData, currentEmployee.EmployeeID),
            };

            return View(model);
        }

        // Post: HistoryOrderProduct/SearchSearchRequest
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

            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);

            var model = new ViewIndex()
            {
                DateEnd = viewData.DateEnd,
                DateStart = viewData.DateStart,
                OrderStatus = viewData.OrderStatus,
                SearchValue = viewData.SearchValue,
                Data = _supportsRepository.LoadSupport(viewData, currentEmployee.EmployeeID),
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _supportsRepository.Count(viewData, currentEmployee.EmployeeID),
            };

            return View(model);
        }

        // GET: Support/Details/5
        [HttpGet]
        [ActionName("Details")]
        [AllowAnonymous]
        public ActionResult Details(long supportsID)
        {
            var currentEmployee = ConvertUtils<Employee>.Deserialize(User.Identity.Name);
            if (currentEmployee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (supportsID.Equals(null))
            {
                return RedirectToAction(actionName: "Index", controllerName: "Support");
            }


            var model = new ViewDetails()
            {
                Supports = _supportsRepository.GetSupports(supportsID),
                SupportsDetail = _supportsRepository.LoadSupportDetail(supportsID, currentEmployee.EmployeeID),
            };

            //if (model.SupportsDetail.Count == 0)
            //{
            //    return RedirectToAction(actionName: "Index", controllerName: "Support");
            //}

            return View(model);
        }

        // POST: Support/Details/Save
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
                return RedirectToAction(ActionDetails, "Support", new { SupportsID = dataDto.Supports.SupportsID });
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
                    

                var isCreated = _supportsRepository.CreateSupportsDetail(dataDto, currentEmployee.EmployeeID);
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

            return RedirectToAction(ActionDetails, "Support", new { SupportsID = dataDto.Supports.SupportsID });
        }

       

        [HttpGet]
        public ActionResult Request_for_feedback()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestDone()
        {
            return View();
        }

        // GET: Support/Create
        [HttpGet]
        public ActionResult Create()
        {
         return View();
        }

        // POST: Support/Create
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewCreate dataDto, HttpPostedFileBase photoUploaded)
        {

            //validate
            if (!ModelState.IsValid)
            {
                return View("Create", dataDto);
            }

            if (dataDto.OderID == ValueIsNull)
            {
                TempData[KeyErrorOderID] = ValueNotOderID;
                return View("Create", dataDto);
            }

            // config upload
            string rootPath = Server.MapPath(STORAGE_ROOT_IMAGES_Detail);
            string path = null;

            // check upload when add product
            if (photoUploaded == null)
            {
                //TempData[KeyErrorMessage] = ValueNotUploadFileWhenAdd;
                //return View("Add", dataDto);

                var currentEmployee1 = ConvertUtils<Employee>.Deserialize(User.Identity.Name);

                var isCreated1 = _supportsRepository.CreateSupports(dataDto, currentEmployee1.EmployeeID);
                if (isCreated1 <= 0)
                {
                    //TempData[KeyErrorMessage] = ValueErrorAddProduct;
                    return View("Add", dataDto);
                }

                return RedirectToAction(ActionDetails, "Support", new { SupportsID = isCreated1 });
            }




            if (photoUploaded != null && photoUploaded.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(photoUploaded.FileName).ToLower();

                if (!UploadFileConstant.AllowedExtensions.Contains(fileExtension)
                    || UploadFileConstant.RejectExtensions.Contains(fileExtension))
                {
                    //TempData[KeyErrorMessage] = ValueErrorUploadFile;
                    return View("Add", dataDto);
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

            var isCreated = _supportsRepository.CreateSupports(dataDto, currentEmployee.EmployeeID);
            if (isCreated <= 0)
            {
                //TempData[KeyErrorMessage] = ValueErrorAddProduct;
                return View("Add", dataDto);
            }

            // }

            // save file uploaed
            if (path != null)
            {
                photoUploaded.SaveAs(path);
            }

            return RedirectToAction(ActionDetails, "Support", new { SupportsID = isCreated });

        }

        // GET: Support/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Support/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Support/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Support/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
