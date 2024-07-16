using Flsurf.Domain.User.Entities;
using Microsoft.AspNetCore.Identity;

namespace Flsurf.Infrastructure
{
    public class PasswordService
    {
        public PasswordHasher<UserEntity> PasswordHasher { get; set; }

        public PasswordService(PasswordHasher<UserEntity> passwordHasher)
        {
            PasswordHasher = passwordHasher;
        }

        public string HashPassword(UserEntity user, string password)
        {
            return PasswordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(UserEntity user, string hashedPassword, string password)
        {
            return PasswordHasher.VerifyHashedPassword(user, hashedPassword, password);
        }
    }
}
