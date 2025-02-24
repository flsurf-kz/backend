using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractQuery : BaseQuery
    {
        public Guid ContractId { get; set; } 
    }
}
