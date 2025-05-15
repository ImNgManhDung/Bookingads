using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.ManageCatelogProduct;
using BookingAds.Areas.Admin.Repository.CatelogProduct;
using BookingAds.Areas.Admin.Repository.CatelogProduct.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Constants;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class ManageCatelogProductController : Controller
    {
        #region Repository
        private readonly ICatelogProductRepository _catelogProductRepo = new CatelogProductRepository();
        #endregion
        #region Constant
        private const int PAGE_SIZE = 5;
        private const string ControllerName = "ManageCatelogProduct";
        private const string ActionIndex = "Index";
        private const string ActionFilter = "Filter";
        private const string ActionAdd = "Add";
        private const string ActionEdit = "Edit";
        private const string ActionDelete = "Delete";
        private const string TitleDeleteCatelogProduct = "Xóa loại quảng cáo";
        private const string MsgCatelogProductIsExists = "Loại Quảng Cáo Đã Tồn Tại";
        private const string MsgCatelogProductNameIsRequired = "Tên Loại Quảng Cáo Không Được Để Trống";
        private const string MsgSuccess = "Thành công";
        private const string MsgCatelogProductIsNotExists = "Loại Quảng cáo Không Tồn Tại";
        private const string MsgHasError = "Có lỗi";
        #endregion
        #region Action

        // GET: Admin/ManageCatelogProduct
        [HttpGet]
        [ActionName(ActionIndex)]
        public ActionResult Index()
        {
            var viewData = new ViewFilterCatelogProduct()
            {
                SearchValue = string.Empty,
                Page = 1,
                PageSize = PAGE_SIZE,
            };

            var model = new ViewManageCatelogProduct()
            {
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _catelogProductRepo.Count(viewData),
                Data = _catelogProductRepo.LoadCatelogProducts(viewData),
            };
            return View(model);
        }

        // POST:  Admin/ManageCatelogProduct/Filter
        [HttpPost]
        [ActionName(ActionFilter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Filter(ViewFilterCatelogProduct viewData)
        {
            if (viewData == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var model = new ViewManageCatelogProduct()
            {
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _catelogProductRepo.Count(viewData),
                Data = _catelogProductRepo.LoadCatelogProducts(viewData),
            };

            // return Json(employees, JsonRequestBehavior.AllowGet);
            return View(model);
        }

        // POST:  Admin/ManageCatelogProduct/Add
        [HttpPost]
        [ActionName(ActionAdd)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Add(ViewCreateCatelogProduct model)
        {
            if (model == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(model.CatelogName))
            {
                return Json(MsgCatelogProductNameIsRequired, JsonRequestBehavior.AllowGet);
            }

            var isCheckCatelogProductIsExists = _catelogProductRepo.CheckCreateCatelogProduct(model.CatelogName);
            if (isCheckCatelogProductIsExists)
            {
                return Json(MsgCatelogProductIsExists, JsonRequestBehavior.AllowGet);
            }

            var isCreated = _catelogProductRepo.CreateCatelogProduct(model);
            if (!isCreated)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        // POST:  Admin/ManageCatelogProduct/Edit
        [HttpPost]
        [ActionName(ActionEdit)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Edit(ViewCreateCatelogProduct dataView)
        {
            if (dataView == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var catelogProduct = _catelogProductRepo.GetCatelogProducts(dataView.CatelogProductsID);

            if (catelogProduct == null)
            {
                return Json(MsgCatelogProductIsNotExists, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(dataView.CatelogName))
            {
                return Json(MsgCatelogProductNameIsRequired, JsonRequestBehavior.AllowGet);
            }

            var isCheckCatelogProductIsExists = _catelogProductRepo.CheckCreateCatelogProduct(dataView.CatelogName);
            if (isCheckCatelogProductIsExists)
            {
                return Json(MsgCatelogProductIsExists, JsonRequestBehavior.AllowGet);
            }

            var isUpdated = _catelogProductRepo.UpdateCatelogProduct(dataView);
            if (!isUpdated)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        // POST:  Admin/ManageCatelogProduct/Delete
        [HttpPost]
        [ActionName(ActionDelete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Delete(long catelogID = 0)
        {
            ViewBag.Title = TitleDeleteCatelogProduct;

            if (catelogID == 0)
            {
                // return RedirectToAction("Index");
                return Json(MsgCatelogProductIsNotExists, JsonRequestBehavior.AllowGet);
            }

            var catelogProduct = _catelogProductRepo.DeleteCatelogProduct(catelogID);

            if (!catelogProduct)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}