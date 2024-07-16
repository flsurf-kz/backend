using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class WarnUser : BaseUseCase<WarnUserDto, UserEntity>
    {
        private IApplicationDbContext Context { get; set; }
        private IAccessPolicy AccessPolicy { get; set; }

        public WarnUser(IApplicationDbContext dbContext,
            IAccessPolicy accessPolicy)
        {
            Context = dbContext;
            AccessPolicy = accessPolicy;
        }

        public async Task<UserEntity> Execute(WarnUserDto dto)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null)
                throw new EntityDoesNotExists(nameof(UserEntity), "");

            user.Warn(dto.Reason, await AccessPolicy.GetCurrentUser());
            await Context.SaveChangesAsync();

            return user;
        }
    }
}
