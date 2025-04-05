﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

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

    public class GetUserHandler : IQueryHandler<GetUserQuery, UserEntity?>
    {
        private readonly IApplicationDbContext _context;

        public GetUserHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> Handle(GetUserQuery query)
        {
            var userQuery = _context.Users
                .IncludeStandard()
                .AsNoTracking()
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
