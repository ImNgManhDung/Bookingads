using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Constants;

namespace BookingAds.Common.Models.Account
{
    public class ViewLogin
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc phải có")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu bắt buộc phải có")]
        [MinLength(ValidatesConstant.MIN_PASSWORD, ErrorMessage = "{0} tối thiểu có {1} ký tự")]
        public string Password { get; set; }
    }
}