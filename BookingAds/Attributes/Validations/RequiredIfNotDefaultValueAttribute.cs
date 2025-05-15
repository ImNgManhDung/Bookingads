using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingAds.Attributes.Validations
{
    public class RequiredIfNotDefaultValueAttribute : ValidationAttribute
    {
        private readonly string _defaultValue;

        public RequiredIfNotDefaultValueAttribute(string defaultValue)
        {
            _defaultValue = defaultValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == _defaultValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}