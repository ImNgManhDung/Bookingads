using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Attributes.Validations;
using BookingAds.Constants;

namespace BookingAds.Common.Models.Account
{
    public class ViewChangePassword
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Mật khẩu cũ bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [MatchCurrentPasswordValidation(ErrorMessage = "{0} không chính xác")]
        public string CurrentPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Mật khẩu mới bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [MustNotBeTheSameValidation(nameof(CurrentPassword), ErrorMessage = "Mật khẩu mới không được giống với mật khẩu hiện tại")]
        public string NewPassword { get; set; }

        [Display(Name = "Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "Nhập lại mật khẩu mới bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [Compare(nameof(NewPassword), ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }

        public string CurrentUserName { get; set; }
    }
}