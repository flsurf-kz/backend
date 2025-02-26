using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetCategoriesQuery : BaseQuery
    {
        public string? SearchQuery { get; set; }
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10;

    }
}
