using Flsurf.Domain.Study.Entities;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class BookQueries
    {
        public static IQueryable<BookEntity> IncludeStandard(this IQueryable<BookEntity> query)
        {
            return query; 
        }
    }
}
