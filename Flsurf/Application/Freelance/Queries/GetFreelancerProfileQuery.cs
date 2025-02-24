using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileQuery : BaseQuery
    {
        public Guid UserId { get; }
    }
}
