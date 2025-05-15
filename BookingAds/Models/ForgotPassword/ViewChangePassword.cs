using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Constants;

namespace BookingAds.Models.ForgotPassword
{
    public class ViewChangePassword
    {
        public ViewChangePassword()
            {
            }

        public string Username { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Mật khẩu mới bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_PASSWORD, MinimumLength = ValidatesConstant.MIN_PASSWORD,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        public string Password { get; set; }

        public string Token { get; set; }
    }
}