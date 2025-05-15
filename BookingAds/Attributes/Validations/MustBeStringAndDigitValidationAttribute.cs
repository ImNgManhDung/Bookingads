using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BookingAds.Constants;

namespace BookingAds.Attributes.Validations
{
    public class MustBeStringAndDigitValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "mustbestringanddigit",
            };

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var regexUpper = new Regex(ValidatesConstant.UPPER);
            var regexLower = new Regex(ValidatesConstant.LOWER);
            var regexDigit = new Regex(ValidatesConstant.DIGIT);
            var data = Convert.ToString(value);

            if (validationContext == null || regexUpper.IsMatch(data) || regexLower.IsMatch(data) || regexDigit.IsMatch(data))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}