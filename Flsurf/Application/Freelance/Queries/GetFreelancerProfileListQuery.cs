using Flsurf.Application.Common.cqrs;
using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileListQuery : BaseQuery
    {
        public int Start { get; set; } = 10; 
        public int Ends { get; set; } = 0;
        public bool YourHires { get; set; } = false; 
        public Countries? Location { get; set; }
        public Guid[]? Skills { get; set; }
        public int[]? CostPerHour { get; set; } // [0] - min, [1] - max
        public int[]? ReviewsCount { get; set; } // [0] - min, [1] - max
    }
}
