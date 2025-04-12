using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetSkillsQuery : BaseQuery
    {
        public string? SearchQuery { get; set; } // Поиск по названию
        public int Starts { get; set; } = 0;
        public int Ends { get; set; } = 50; 
    }
}
