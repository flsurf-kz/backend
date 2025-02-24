using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileListQuery : BaseQuery
    {
        public int Start { get; }
        public int Ends { get; }
        public string[]? Skills { get; }
        public string[]? Languages { get; }
        public int[]? CostPerHour { get; } // [0] - min, [1] - max
        public int[]? ReviewsCount { get; } // [0] - min, [1] - max
    }
}
