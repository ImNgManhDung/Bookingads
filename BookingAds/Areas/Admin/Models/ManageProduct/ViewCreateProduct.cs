using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Attributes.Validations;
using BookingAds.Constants;

namespace BookingAds.Areas.Admin.Models.ManageProduct
{
    public class ViewCreateProduct
    {
        public long ProductID { get; set; }

        [Display(Name = "Tên quảng cáo")]
        [Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
        [StringLength(ValidatesConstant.MAX_FOOD_NAME, MinimumLength = ValidatesConstant.MIN_FOOD_NAME,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [MustBeStringAndDigitValidationAttribute(ErrorMessage = "{0} yêu cầu phải là kiểu ký tự dạng chữ hoặc dạng số")]

        public string ProductName { get; set; }

        [Display(Name = "Số lượng tồn")]
        [Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
        [Range(ValidatesConstant.MIN_QUANTITY_FOOD, ValidatesConstant.MAX_QUANTITY_FOOD, ErrorMessage = "{0} phải là số dương và thuộc {1} đến {2}")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
        [Range(ValidatesConstant.MIN_PRICE_FOOD, ValidatesConstant.MAX_PRICE_FOOD, ErrorMessage = "{0} phải là số dương và thuộc {1} đến {2}")]
        public decimal Price { get; set; }

        [Display(Name = "Loại đồ ăn")]
        [Required(ErrorMessage = "Yêu chọn dữ liệu {0}")]
        [MustBeSelectListValidation(0, ErrorMessage = "Phải chọn Loại đồ ăn khác với giá trị mặc định")]
        public long CatelogProductsID { get; set; }

        public string Photo { get; set; }
    }
}