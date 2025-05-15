using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAds.Common.Repository.Account;
using BookingAds.Entities;
using BookingAds.Modules;

namespace BookingAds.Attributes.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MatchCurrentPasswordValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "matchcurrentpassword",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            };
            yield return modelClientValidationRule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentUser = ConvertUtils<Account>.Deserialize(HttpContext.Current.User.Identity.Name);
            var inputCurrentPassword = Convert.ToString(value);
            var hashInputCurrentPassword = PasswordUtils.MD5(inputCurrentPassword);

            if (currentUser == null || (currentUser != null && hashInputCurrentPassword != currentUser.Password))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            // return currentUser != null && hashInputCurrentPassword == currentUser.Password;
            return ValidationResult.Success;
        }
    }
}