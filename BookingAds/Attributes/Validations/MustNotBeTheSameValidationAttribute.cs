using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAds.Attributes.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MustNotBeTheSameValidationAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherProperty;

        public MustNotBeTheSameValidationAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        public override bool IsValid(object value)
        {
            var data = Convert.ToString(value);
            return data != null && data != _otherProperty;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "mustnotbethesame",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            };
            modelClientValidationRule.ValidationParameters["elementname"] = _otherProperty;
            yield return modelClientValidationRule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectType.GetProperty(_otherProperty);
            var dataOtherProperty = Convert.ToString(otherProperty.GetValue(validationContext.ObjectInstance));
            var data = Convert.ToString(value);

            if (data == null || dataOtherProperty == null || (data != null && dataOtherProperty != null && data == dataOtherProperty))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}