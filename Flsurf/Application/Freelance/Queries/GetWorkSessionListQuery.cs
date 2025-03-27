using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetWorkSessionListQuery : BaseQuery
    {
        public required Guid ContractId { get; set; } // ID контракта, снэпшоты которого нужно получить
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10; 
    }

}
