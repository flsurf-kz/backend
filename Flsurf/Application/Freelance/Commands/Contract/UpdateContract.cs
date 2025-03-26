using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class UpdateContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ContractTerms { get; set; }
        public decimal? Bonus { get; set; }
    }

    public class UpdateContract
    {
    }
}
