using Microsoft.EntityFrameworkCore;
using Flsurf.Infrastructure;
using Flsurf.Domain.User.Enums;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.User.Dto;
using Flsurf.Application.Common.Exceptions;

namespace Flsurf.Application.User.UseCases
{
    public class CreateUser : BaseUseCase<CreateUserDto, UserEntity>
    {
        private IApplicationDbContext _context;
        private PasswordService passwordService;

        public CreateUser(
            IApplicationDbContext dbContext,
            PasswordService pswdService)
        {
            passwordService = pswdService;
            _context = dbContext;
        }

        public async Task<UserEntity> Execute(CreateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.EmailAddress);
            if (user != null)
            {
                throw new EntityAlreadyExists(nameof(UserEntity), dto.EmailAddress, "EmailAddress");
            }

            var newUser = UserEntity.Create(
                fullname: dto.Surname + " " + dto.Name,
                email: dto.EmailAddress,
                password: dto.Password,
                userType: dto.UserType, 
                passwordService: passwordService
            );

            _context.Users.Add(newUser);
            // sends event to freelance module to crete freelancer or client for this user
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}
