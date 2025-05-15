using BookingAds.Attributes.Validations;
using BookingAds.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingAds.Models.Register
{
    public class ViewRegister
    {
        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Họ bắt buộc phải có")]
        public string FirstName { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Tên bắt buộc phải có")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email bắt buộc phải có")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại cá nhân")]
        [Required(ErrorMessage = "Số điện thoại cá nhân bắt buộc phải có")]
        [StringLength(ValidatesConstant.PHONE_LENGTH, MinimumLength = ValidatesConstant.PHONE_LENGTH,
            ErrorMessage = "{0} phải đủ {1} kí tự")]
        public string Phone { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Giới tính của khách hàng bắt buộc phải có")]
        public int Gender { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu ")]
        [Required(ErrorMessage = "Nhập lại mật khẩu bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }

    }
}