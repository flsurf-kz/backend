using Microsoft.EntityFrameworkCore;
using Flsurf.Infrastructure;
using Flsurf.Domain.User.Enums;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.User.Dto;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.cqrs;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class CreateUserCommand : BaseCommand
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        public string? Phone { get; set; }
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public UserTypes UserType { get; set; }
        public UserRoles? Role { get; set; }
    }

    public class CreateUserHandler(IApplicationDbContext _context, PasswordService _passwordService) : ICommandHandler<CreateUserCommand>
    {
        public async Task<CommandResult> Handle(CreateUserCommand command)
        {
            // Проверяем, существует ли уже пользователь с таким Email
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            if (existingUser != null)
            {
                return CommandResult.Conflict($"Пользователь с email {command.Email} уже существует.");
            }

            // Создаем нового пользователя
            var newUser = UserEntity.Create(
                fullname: $"{command.Surname} {command.Name}",
                email: command.Email,
                password: command.Password,
                userType: command.UserType,
                passwordService: _passwordService
            );

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CommandResult.Success(newUser.Id);
        }
    }
}
