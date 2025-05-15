using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAds.Attributes.Validations
{
    public class MustBeSelectListValidationAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly object _notAllowValue;

        public MustBeSelectListValidationAttribute(object notAllowValue)
        {
            _notAllowValue = notAllowValue;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "mustbeselectlist",
            };

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = Convert.ToString(value);
            var notAllowValue = Convert.ToString(_notAllowValue);

            if (data == null || notAllowValue == null || (data != null && notAllowValue != null && data == notAllowValue))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}