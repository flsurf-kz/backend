using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetJobQuery : BaseQuery
    {
        public Guid JobId { get; set; }
    }
}
