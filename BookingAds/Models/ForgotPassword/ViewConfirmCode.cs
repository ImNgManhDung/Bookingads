using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Constants;

namespace BookingAds.Models.ForgotPassword
{
    public class ViewConfirmCode
    {
        public ViewConfirmCode()
        {
        }

        public string Username { get; set; }

        [Display(Name = "Nhập mã xác minh")]
        [Required(ErrorMessage = "Bắt buộc nhập mã xác minh")]
        [StringLength(ValidatesConstant.DFCODE, ErrorMessage = "Mã xác minh chỉ có {1} ký tự ", MinimumLength = ValidatesConstant.DFCODE)]
        public string ConfirmCode { get; set; }

        public string Token { get; set; }
    }
}