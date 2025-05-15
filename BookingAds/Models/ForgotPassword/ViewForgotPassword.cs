using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingAds.Models.ForgotPassword
{
    public class ViewForgotPassword
    {
        public ViewForgotPassword()
        {
        }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc phải có")]
        public string Username { get; set; }
    }
}