using Flsurf.Domain.Common;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Tests.Domain.UnitTests.Entities
{
    [TestFixture]
    public class UserEntityTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void SetUp()
        {
            _passwordService = new PasswordService(new PasswordHasher<UserEntity>());
        }

        [Test]
        public void Create_WithValidParameters_ShouldReturnValidUser()
        {
            // Arrange
            var fullname = "Иван Иванов";
            var email = "ivan@example.com";
            var password = "securePassword123";
            var type = UserTypes.Client;

            // Act
            var user = UserEntity.Create(fullname, email, password, type, _passwordService);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Fullname, Is.EqualTo(fullname));
            Assert.That(user.Email, Is.EqualTo(email));
            Assert.That(user.Type, Is.EqualTo(type));
            Assert.That(user.HashedPassword, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Block_ShouldSetBlockedToTrue()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "password", UserTypes.Client, _passwordService);

            user.Block();

            Assert.That(user.Blocked, Is.True);
        }

        [Test]
        public void UpdatePassword_WithCorrectOldPassword_ShouldUpdatePassword()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "oldPass", UserTypes.Client, _passwordService);

            user.UpdatePassword("oldPass", "newPass", _passwordService);

            var result = _passwordService.VerifyHashedPassword(user, user.HashedPassword, "newPass");
            Assert.That(result, Is.EqualTo(PasswordVerificationResult.Success));
        }

        [Test]
        public void UpdatePassword_WithIncorrectOldPassword_ShouldThrowAccessDenied()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "correctPass", UserTypes.Client, _passwordService);

            Assert.Throws<AccessDenied>(() =>
                user.UpdatePassword("wrongPass", "newPass", _passwordService));
        }

        [Test]
        public void SetRole_ShouldChangeUserRole()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "pass", UserTypes.Client, _passwordService);

            user.SetRole(UserRoles.Admin);

            Assert.That(user.Role, Is.EqualTo(UserRoles.Admin));
        }

        [Test]
        public void SetTelegramId_WhenNotBlocked_ShouldSetValue()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "pass", UserTypes.Client, _passwordService);

            user.TelegramId = "tg_123";

            Assert.That(user.TelegramId, Is.EqualTo("tg_123"));
        }

        [Test]
        public void SetTelegramId_WhenBlocked_ShouldThrowAccessDenied()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "pass", UserTypes.Client, _passwordService);
            user.Block();

            Assert.Throws<AccessDenied>(() => user.TelegramId = "tg_fail");
        }

        [Test]
        public void Warn_ShouldAddWarningAndNotBlockIfLessThanThree()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "pass", UserTypes.Client, _passwordService);
            var by = UserEntity.Create("Админ Админов", "admin@example.com", "pass", UserTypes.Admin, _passwordService);

            user.Warn("Первое предупреждение", by);

            Assert.That(user.Warnings.Count, Is.EqualTo(1));
            Assert.That(user.Blocked, Is.False);
        }

        [Test]
        public void Warn_ShouldBlockUserIfMoreThanThreeWarnings()
        {
            var user = UserEntity.Create("Иван Иванов", "test@example.com", "pass", UserTypes.Client, _passwordService);
            var by = UserEntity.Create("Админ Админов", "admin@example.com", "pass", UserTypes.Admin, _passwordService);

            user.Warn("1", by);
            user.Warn("2", by);
            user.Warn("3", by);
            user.Warn("4", by); // должно заблокировать

            Assert.That(user.Blocked, Is.True);
        }

        [Test]
        public void Fullname_GetterAndSetter_ShouldWorkCorrectly()
        {
            var user = new UserEntity();
            user.Fullname = "Иван Иванов";

            Assert.That(user.Name, Is.EqualTo("Иванов"));
            Assert.That(user.Surname, Is.EqualTo("Иван"));
            Assert.That(user.Fullname, Is.EqualTo("Иван Иванов"));
        }

        [Test]
        public void Fullname_Setter_WithInvalidFormat_ShouldThrowException()
        {
            var user = new UserEntity();

            Assert.Throws<ArgumentException>(() => user.Fullname = "WrongFormatNameOnly");
        }
    }
}
