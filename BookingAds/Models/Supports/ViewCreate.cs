using BookingAds.Attributes.Validations;
using BookingAds.Constants;
using BookingAds.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace BookingAds.Models.Supports
{
    public class ViewCreate
    {
        public CatelogProduct CatelogProduct { get; set; }

        //[Display(Name = "Tên đăng nhập")]
        //[Required(ErrorMessage = "Tên đăng nhập bắt buộc phải có")]
        public string To { get; set; }

        [Display(Name = "Tiêu đề câu hỏi")]
        [Required(ErrorMessage = "Tiêu đề bắt buộc phải có")]
        [MinLength(ValidatesConstant.MIN_PASSWORD, ErrorMessage = "{0} tối thiểu có {1} ký tự")]
        public string Subject { get; set; }

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung bắt buộc phải có")]
        [MinLength(ValidatesConstant.MIN_PASSWORD, ErrorMessage = "{0} tối thiểu có {1} ký tự")]
        public string SubjectMessener { get; set; }
        public string Images { get; set; }

        [Display(Name = "Dịch vụ")]
        [RequiredIfNotDefaultValue("-99", ErrorMessage = "Phải chọn dịch vụ")]
        public string OderID { get; set; }


    }
}