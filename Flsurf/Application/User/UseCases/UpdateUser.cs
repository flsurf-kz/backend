using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class UpdateUser : BaseUseCase<UpdateUserDto, UserEntity>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;
        private PasswordService _passwordService;
        private IFileService _fileService;

        public UpdateUser(
            IApplicationDbContext context,
            IPermissionService permService,
            PasswordService passwordService,
            IUser user,
            IFileService fileService)
        {
            _fileService = fileService;
            _passwordService = passwordService;
            _context = context;
            _permService = permService;
        }

        public async Task<UserEntity> Execute(UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);

            Guard.Against.Null(user, message: "User does not exists");

            var byUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            Guard.Against.Null(byUser, message: "User does not exists");

            // TODO
            //if (byUser.Id != user.Id && !await _permService.IsAllowed(PermissionEnum.edit, user, byUser))
            //{
            //    throw new AccessDenied("User access denied"); 
            //}
            if (dto.Email != null)
            {
                user.Email = dto.Email;
            }
            if (dto.Name != null)
            {
                user.Name = dto.Name;
            }
            if (dto.Surname != null)
            {
                user.Surname = dto.Surname; 
            }
            if (dto.TelegramId != null)
            {
                user.TelegramId = dto.TelegramId;
            }
            if (dto.NewPassword != null && dto.OldPassword != null)
            {
                user.UpdatePassword(
                    oldPassword: dto.OldPassword,
                    newPassword: dto.NewPassword,
                    passwordService: _passwordService);
            }
            if (dto.Role != null)
            {
                await _permService.EnforceCheckPermission(
                    ZedUser.WithId(byUser.Id).CanUpdateRole(ZedUser.WithId(user.Id))); 

                user.SetRole((UserRoles)dto.Role);
            }
            if (dto.Avatar != null)
            {
                user.Image = await _fileService.UploadFile().Execute(dto.Avatar);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
