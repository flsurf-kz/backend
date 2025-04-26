using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Common.Models
{
    public class GreaterThanZeroAttribute : RangeAttribute
    {
        public GreaterThanZeroAttribute()
            : base(typeof(double), "0.0000001", double.MaxValue.ToString())
        {
            ErrorMessage = "Значение должно быть больше нуля";
        }
    }
}
