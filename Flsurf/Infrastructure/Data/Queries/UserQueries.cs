using Flsurf.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class UserQueries
    {
        public static IQueryable<UserEntity> IncludeStandard(this IQueryable<UserEntity> query)
        {
            return query
                .Include(x => x.Avatar);
        }
    }
}
