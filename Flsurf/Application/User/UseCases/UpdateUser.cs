using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Infrastructure;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class UpdateUser : BaseUseCase<UpdateUserDto, bool>
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

        public async Task<bool> Execute(UpdateUserDto dto)
        {
            var user = await _context.Users
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.UserId);

            Guard.Against.Null(user, message: "User does not exists");

            var byUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            Guard.Against.Null(byUser, message: "User does not exists");

            if (byUser.Id != user.Id)
            {
                throw new AccessDenied("You are not user");
            }
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
            if (dto.Avatar != null)
            {
                user.Image = await _fileService.UploadFile().Execute(dto.Avatar);
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }

    public class UpdateUserRole : BaseUseCase<UpdateUserRoleDto, bool>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;

        public UpdateUserRole(
            IApplicationDbContext context,
            IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<bool> Execute(UpdateUserRoleDto dto)
        {
            var user = await _context.Users
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.UserId);

            Guard.Against.NotFound(dto.UserId, user);

            var byUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            Guard.Against.NotFound(user.Id, byUser);

            await _permService.EnforceCheckPermission(
                ZedUser.WithId(byUser.Id).CanUpdateRole(ZedUser.WithId(user.Id)));

            user.SetRole(dto.Role);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
