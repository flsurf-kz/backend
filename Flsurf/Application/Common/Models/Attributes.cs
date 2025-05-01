using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Flsurf.Application.Common.Models
{
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public GreaterThanZeroAttribute()
        {
            ErrorMessage = "Значение должно быть больше нуля";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            // null (не обязательно) пропускаем:
            if (value is null)
                return ValidationResult.Success;

            // попытаемся привести к decimal:
            if (!decimal.TryParse(value.ToString(),
                                  NumberStyles.Number,
                                  CultureInfo.CurrentCulture,
                                  out var amount))
            {
                return new ValidationResult("Неверный формат числа");
            }

            return amount > 0
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage!);
        }
    }

}
