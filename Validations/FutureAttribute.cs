using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Validations
{
    public class FutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value < DateTime.Now)
            {
                return new ValidationResult("Event must in the future");
            }
            return ValidationResult.Success;
        }
    }
}