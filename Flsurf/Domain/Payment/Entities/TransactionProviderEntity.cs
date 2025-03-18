using Flsurf.Domain.Common;
using Flsurf.Domain.Files.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Payment.Entities
{
    public class TransactionProviderEntity : BaseAuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal FeePercent { get; set; } = 0;

        [Required]
        public List<PaymentSystemEntity> Systems { get; set; } = new List<PaymentSystemEntity>();

        [Required]
        public FileEntity Logo { get; set; } = null!;

        // Возвращает активные платежные системы у данного провайдера
        public IEnumerable<PaymentSystemEntity> GetActiveSystems() =>
            Systems.Where(system => system.IsActive);

        public void AddPaymentSystem(PaymentSystemEntity system)
        {
            if (Systems.Any(s => s.Name == system.Name))
                throw new InvalidOperationException($"Payment system '{system.Name}' already exists.");

            Systems.Add(system);
        }

        public void RemovePaymentSystem(string systemName)
        {
            var system = Systems.FirstOrDefault(s => s.Name == systemName);
            if (system == null)
                throw new InvalidOperationException($"Payment system '{systemName}' not found.");

            Systems.Remove(system);
        }
    }

}
