using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Areas.Admin.Models.ManageProduct;
using BookingAds.Areas.Admin.Repository.CatelogProduct;
using BookingAds.Areas.Admin.Repository.CatelogProduct.Abstractions;
using BookingAds.Areas.Admin.Repository.Product;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Attributes.Filters;
using BookingAds.Constants;

namespace BookingAds.Areas.Admin.Controllers
{
    [RoleFilter(Roles = RoleConstant.ADMIN)]
    public class ManageProductController : Controller
    {
        #region Repository
        private readonly ICatelogProductRepository _catelogProductRepo = new CatelogProductRepository();
        private readonly IProductRepository _productRepo = new ProductRepository();
        #endregion
        #region Constant
        private const string STORAGE_ROOT_IMAGES_FOOD = "~/Images/Products";
        private const int PAGE_SIZE = 8;
        private const string KeyErrorMessage = "error_message";
        private const string ControllerName = "ManageProduct";
        private const string ActionIndex = "Index";
        private const string ActionFilter = "Filter";
        private const string ActionAdd = "Add";
        private const string ActionEdit = "Edit";
        private const string ActionSave = "Save";
        private const string ActionSoftDelete = "SoftDelete";
        private const string ActionRestore = "Restore";
        private const string TitleAddProduct = "Thêm Quảng Cáo";
        private const string TitleEditProduct = "Sửa Quảng Cáo";
        private const string ValueErrorUploadFile = "Tệp tải lên phải thuộc các loại sau: .png, .jpg, .gif, .jpeg, .tiff";
        private const string ValueErrorAddProduct = "Có lỗi xảy ra khi thêm mới quảng cáo";
        private const string ValueErrorEditProduct = "Có lỗi xảy ra khi sửa quảng cấo";
        private const string ValueNotUploadFileWhenAdd = "Hình ảnh bắt buộc phải có";
        private const string MsgProductIsNotExists = "Quảng cáo không tồn tại";
        private const string MsgHasError = "Có lỗi";
        private const string MsgFail = "Thất bại";
        private const string MsgSuccess = "Thành công";
        #endregion
        #region Action

        // GET: Admin/ManageProduct
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
            var model = new ViewManageBookingAds()
            {
                Data = _productRepo.GetProducts(viewData),
                Sort = $"{viewData.SortField}-{viewData.SortType}",
                SortField = viewData.SortField,
                SortType = viewData.SortType,
                CatelogProductsID = viewData.CatelogProductsID,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _productRepo.Count(viewData),
            };
            return View(model);
        }

        // POST: Admin/ManageProduct/Filter
        [HttpPost]
        [ActionName(ActionFilter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Filter(ViewFilterBookingAds viewData)
        {
            if (viewData == null)
            {
                return Json(MsgHasError, JsonRequestBehavior.AllowGet);
            }

            var model = new ViewManageBookingAds()
            {
                Data = _productRepo.GetProducts(viewData),
                Sort = $"{viewData.SortField}-{viewData.SortType}",
                SortField = viewData.SortField,
                SortType = viewData.SortType,
                CatelogProductsID = viewData.CatelogProductsID,
                SearchValue = viewData.SearchValue,
                CurrentPage = viewData.Page,
                CurrentPageSize = viewData.PageSize,
                TotalRow = _productRepo.Count(viewData),
            };
            return View(model);
        }

        // GET: Admin/ManageProduct/Add
        [HttpGet]
        [ActionName(ActionAdd)]
        public ActionResult Add()
        {
            ViewBag.Title = TitleAddProduct;
            var model = new ViewCreateProduct()
            {
                ProductID = 0,
            };
            return View(model);
        }

        // GET: Admin/ManageProduct/Edit/:id
        [HttpGet]
        [ActionName(ActionEdit)]
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Title = TitleEditProduct;

            if (id == 0)
            {
                return RedirectToAction(ActionIndex, ControllerName);
            }

            var product = _productRepo.GetProduct(id);
            var model = new ViewCreateProduct()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Quantity = product.Quantity,
                Price = product.Price,
                CatelogProductsID = product.CatelogProduct.CatelogProductsID,
                Photo = product.Photo,
            };
            return View(ActionAdd, model);
        }

        // POST: Admin/ManageProduct/Save
        [HttpPost]
        [ActionName(ActionSave)]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ViewCreateProduct dataDto, HttpPostedFileBase photoUploaded)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(ActionAdd, dataDto);
            }

            // config upload
            string rootPath = Server.MapPath(STORAGE_ROOT_IMAGES_FOOD);
            string path = null;

            // check upload when add product
            if (photoUploaded == null && dataDto.ProductID == 0)
            {
                TempData[KeyErrorMessage] = ValueNotUploadFileWhenAdd;
                return View(ActionAdd, dataDto);
            }

            if (photoUploaded != null && photoUploaded.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(photoUploaded.FileName).ToLower();

                if (!UploadFileConstant.AllowedExtensions.Contains(fileExtension)
                    || UploadFileConstant.RejectExtensions.Contains(fileExtension))
                {
                    TempData[KeyErrorMessage] = ValueErrorUploadFile;
                    return View(ActionAdd, dataDto);
                }

                var uniqueFileName = $"{DateTime.Now.Ticks}_{photoUploaded.FileName}";

                dataDto.Photo = uniqueFileName;

                // create storage path
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                path = Path.Combine(rootPath, Path.GetFileName(dataDto.Photo));
            }

            // create new product
            if (dataDto.ProductID == 0)
            {
                var isCreated = _productRepo.CreateProduct(dataDto);
                if (!isCreated)
                {
                    TempData[KeyErrorMessage] = ValueErrorAddProduct;
                    return View(ActionAdd, dataDto);
                }
            }

            // update product
            else
            {
                if (string.IsNullOrWhiteSpace(dataDto.Photo))
                {
                    dataDto.Photo = _productRepo.GetProduct(dataDto.ProductID).Photo;
                }

                var isUpdated = _productRepo.UpdateProduct(dataDto);
                if (!isUpdated)
                {
                    TempData[KeyErrorMessage] = ValueErrorEditProduct;
                    return View(ActionAdd, dataDto);
                }
            }

            // save file uploaed
            if (path != null)
            {
                photoUploaded.SaveAs(path);
            }

            return RedirectToAction(ActionIndex);
        }

        // POST: Admin/ManageProduct/SoftDelete
        [HttpPost]
        [ActionName(ActionSoftDelete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult SoftDelete(long productID = 0)
        {
            if (productID == 0)
            {
                return Json(MsgProductIsNotExists, JsonRequestBehavior.AllowGet);
            }

            var isSoftDelete = _productRepo.SoftDeleteProduct(productID);
            if (!isSoftDelete)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/ManageProduct/Restore
        [HttpPost]
        [ActionName(ActionRestore)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3147:Mark Verb Handlers With Validate Antiforgery Token", Justification = "<Pending>")]
        public ActionResult Restore(long productID = 0)
        {
            if (productID == 0)
            {
                return Json(MsgProductIsNotExists, JsonRequestBehavior.AllowGet);
            }

            var isRestore = _productRepo.RestoreProduct(productID);
            if (!isRestore)
            {
                return Json(MsgFail, JsonRequestBehavior.AllowGet);
            }

            return Json(MsgSuccess, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}