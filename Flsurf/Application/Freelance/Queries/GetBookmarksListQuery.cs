using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetBookmarksListQuery : BaseQuery
    {
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10;
        //public Guid? UserId { get; set; }
    }
}
