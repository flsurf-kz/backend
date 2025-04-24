using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetPortfolioProjectsQuery : BaseQuery
    {
        public Guid? FreelancerId { get; set; } // ID фрилансера
        public int Start { get; set; } = 0; // Пагинация: откуда начинать
        public int Ends { get; set; } = 10; // Пагинация: до какого элемента
    }
}
