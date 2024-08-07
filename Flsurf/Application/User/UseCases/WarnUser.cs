﻿using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class WarnUser : BaseUseCase<WarnUserDto, UserEntity>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public WarnUser(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<UserEntity> Execute(WarnUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);

            Guard.Against.NotFound(dto.UserId, user); 

            await _permService.EnforceCheckPermission(
                ZedUser
                    .WithId((await _permService.GetCurrentUser()).Id)
                    .CanCreateWarnings()
            ); 

            user.Warn(dto.Reason, await _permService.GetCurrentUser());
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
