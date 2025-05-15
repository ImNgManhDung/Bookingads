using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookingAds.Constants;

namespace BookingAds.Common.Models.Account
{
    public class ViewUpdateEmployeeInfo
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc phải có")]
        public string UserName { get; set; }

        [Display(Name = "Họ và tên đệm")]
        [Required(ErrorMessage = "Họ và tên đệm bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_LASTNAME, MinimumLength = ValidatesConstant.MIN_LASTNAME,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        public string LastName { get; set; }

        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Tên của khách hàng bắt buộc phải có")]
        [StringLength(ValidatesConstant.MAX_FIRSTNAME, MinimumLength = ValidatesConstant.MIN_FIRSTNAME,
            ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
        public string FirstName { get; set; }

        [Display(Name = "Số điện thoại cá nhân")]
        [Required(ErrorMessage = "Số điện thoại cá nhân bắt buộc phải có")]
        [StringLength(ValidatesConstant.PHONE_LENGTH, MinimumLength = ValidatesConstant.PHONE_LENGTH,
            ErrorMessage = "{0} phải đủ {1} kí tự")]
        public string Phone { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Giới tính của khách hàng bắt buộc phải có")]
        public int Gender { get; set; }

        public long Coin { get; set; }

        public string Avatar { get; set; }
    }
}