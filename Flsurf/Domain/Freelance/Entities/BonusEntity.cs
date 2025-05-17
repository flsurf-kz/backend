using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{
    public class BonusEntity : BaseAuditableEntity
    {
        [JsonIgnore]
        public ContractEntity Contract { get; set; } = null!; 
        public Guid ContractId { get; set; }

        public Money Amount { get; set; } = null!;
        public string Description { get; set; } = null!; 
        public BonusType Type { get; set; }

        public static BonusEntity Create(Guid contractId, Money amount, string description, BonusType type)
        {

            return new BonusEntity()
            {
                ContractId = contractId,
                Amount = amount,
                Description = description,
                Type = type
            }; 
        }
    }
}
