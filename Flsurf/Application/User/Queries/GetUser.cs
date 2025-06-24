using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Flsurf.Application.User.Queries
{
    public class GetUserQuery : BaseQuery
    {
        /// <summary>
        /// Идентификатор пользователя (опционально).
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Email пользователя (опционально).
        /// </summary>
        public string? Email { get; set; }
    }

    public class GetUserHandler(IApplicationDbContext context, IPermissionService permService) : IQueryHandler<GetUserQuery, UserEntity?>
    {

        public async Task<UserEntity?> Handle(GetUserQuery query)
        {
            if (query.UserId == null)
            {
                try
                {
                    query.UserId = (await permService.GetCurrentUser()).Id;
                }
                catch (AccessDenied) { }
            }

            var userQuery = context.Users
                .IncludeStandard()
                .AsQueryable();

            if (query.UserId != null)
            {
                userQuery = userQuery.Where(x => x.Id == query.UserId);
            }
            if (!string.IsNullOrEmpty(query.Email))
            {
                userQuery = userQuery.Where(x => x.Email == query.Email);
            }

            var user = await userQuery.FirstOrDefaultAsync();

            return user;
        }
    }
}
