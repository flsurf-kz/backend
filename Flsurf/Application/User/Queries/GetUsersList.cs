using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Queries
{
    public class GetUsersListQuery : BaseQuery
    {
        public string? Fullname { get; set; }
        public UserRoles? Role { get; set; }
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10; 
    }

    public class GetUsersListHandler : IQueryHandler<GetUsersListQuery, List<UserEntity>>
    {
        private readonly IApplicationDbContext _context;

        public GetUsersListHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserEntity>> Handle(GetUsersListQuery query)
        {
            var usersQuery = _context.Users
                .IncludeStandard() // расширение для Include стандартных навигационных свойств
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Fullname))
            {
                usersQuery = usersQuery.Where(x => x.Fullname == query.Fullname);
            }
            if (query.Role != null)
            {
                usersQuery = usersQuery.Where(x => x.Role == query.Role);
            }

            usersQuery = usersQuery.Paginate(query.Start, query.Ends); 

            return await usersQuery.ToListAsync();
        }
    }
}
