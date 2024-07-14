using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class DeleteUser : BaseUseCase<DeleteUserDto, UserEntity>
    {
        private IApplicationDbContext Context { get; set; }
        private IAccessPolicy AccessPolicy { get; set; }

        public DeleteUser(IApplicationDbContext dbContext,
            IAccessPolicy accessPolicy)
        {
            Context = dbContext;
            AccessPolicy = accessPolicy;
        }

        public async Task<UserEntity> Execute(DeleteUserDto dto)
        {
            await AccessPolicy.EnforceRole(UserRoles.Admin); 

            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null)
                throw new EntityDoesNotExists(nameof(UserEntity), "");

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

            return user;
        }
    }
}
