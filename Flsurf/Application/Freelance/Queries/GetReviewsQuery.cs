using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetReviewsQuery : BaseQuery
    {
        public Guid UserId { get; set; }
        public int Starts { get; set; } = 0;
        public int Ends { get; set; } = 20; 
    }
}
