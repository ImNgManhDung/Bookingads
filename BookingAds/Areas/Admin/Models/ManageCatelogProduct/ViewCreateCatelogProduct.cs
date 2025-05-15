using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Attributes.Validations;
using BookingAds.Constants;

namespace BookingAds.Areas.Admin.Models.ManageCatelogProduct
{
    public class ViewCreateCatelogProduct
    {
        public long CatelogProductsID { get; set; }

        [Display(Name = "Loại quảng cáo")]
        [Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
        [StringLength(ValidatesConstant.MAX_FOOD_NAME, MinimumLength = ValidatesConstant.MIN_FOOD_NAME,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [MustBeStringAndDigitValidationAttribute(ErrorMessage = "{0} yêu cầu phải là kiểu ký tự dạng chữ hoặc dạng số")]
        public string CatelogName { get; set; }
    }
}