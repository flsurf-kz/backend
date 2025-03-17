using Flsurf.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Payment.Entities
{
    public class PaymentSystemEntity : BaseAuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public PaymentSystemEntity(string name)
        {
            Name = name;
            IsActive = true;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
