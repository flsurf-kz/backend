using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetTasksQuery : BaseQuery
    {
        public required Guid ContractId { get; set; } // ID контракта, задачи которого нужно получить
        public int Start {  get; set; }
        public int Ends { get; set; }
    }
}
