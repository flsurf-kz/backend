using Flsurf.Application.Common.Exceptions;
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
    public class BlockUser : BaseUseCase<BlockUserDto, UserEntity>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public BlockUser(IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<UserEntity> Execute(BlockUserDto dto)
        {
            await _permService.CheckPermission(
                ZedUser
                    .WithId((await _permService.GetCurrentUser()).Id)
                    .CanDeactivateUser(ZedUser.WithId(dto.UserId)); 

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null)
                throw new EntityDoesNotExists(nameof(UserEntity), "");

            user.Block(); 
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
