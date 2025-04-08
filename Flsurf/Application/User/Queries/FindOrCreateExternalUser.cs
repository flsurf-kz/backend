using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Queries
{
    // осталяем потому что пиздец 
    public class FindOrCreateExternalUser(
            IApplicationDbContext _context,
            PasswordService _passwordService,
            UploadFile _uploadFile) : BaseUseCase<ExternalUserDto, UserEntity>
    {
        public async Task<UserEntity> Execute(ExternalUserDto dto)
        {
            // Ищем существующего пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user != null)
            {
                // Обновляем информацию, если необходимо
                await UpdateUserIfNeeded(user, dto);
                return user;
            }

            // Создаем нового пользователя
            var newUser = UserEntity.CreateExternal(
                fullname: dto.FullName,
                email: dto.Email,
                passwordService: _passwordService);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        private async Task UpdateUserIfNeeded(UserEntity user, ExternalUserDto dto)
        {
            var needsUpdate = false;

            // Обновляем имя, если оно изменилось
            if (!string.Equals(user.Fullname, dto.FullName, StringComparison.OrdinalIgnoreCase))
            {
                user.Fullname = dto.FullName;
                needsUpdate = true;
            }

            // Обновляем аватар
            if (!string.IsNullOrEmpty(dto.AvatarUrl) && (user.Avatar == null || user.Avatar.FilePath != dto.AvatarUrl))
            {
                user.Avatar = await _uploadFile.Execute(new CreateFileDto() { DownloadUrl = dto.AvatarUrl });
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                _context.Users.Update(user);
            }
        }
    }
}
