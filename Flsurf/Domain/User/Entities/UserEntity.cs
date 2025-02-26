using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.User.Entities
{
    public class UserEntity : BaseAuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required, NotMapped]
        public string Fullname
        {
            get
            {
                return $"{Surname} {Name}";
            }
            set
            {
                var parts = value.Split(' ');
                if (parts.Length == 2)
                {
                    Surname = parts[0];
                    Name = parts[1];
                }
                else
                {
                    // В этом случае можно выбросить исключение, 
                    // или выполнить другие действия в зависимости от логики приложения.
                    throw new ArgumentException("Неверный формат Fullname. Имя и фамилия должны быть разделены пробелом.");
                }
            }
        }
        [Required, JsonIgnore]
        public string HashedPassword { get; set; } = null!;
        public UserRoles Role { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Email address is not correct")]
        public string Email { get; set; } = null!;
        [Column(name: "TelegramId")]
        private string? _telegramId; // Закрытое поле для хранения значения
        public FileEntity? Avatar { get; set; }
        [Required]
        public bool IsOnline { get; set; } = false;
        [JsonIgnore]
        public ICollection<WarningEntity> Warnings { get; set; } = [];
        public bool IsSuperadmin { get; set; } = false;
        [Phone]
        public string? Phone { get; set; } 

        [NotMapped]
        public string? TelegramId
        {
            get => _telegramId; // Возвращаем значение из закрытого поля
            set
            {
                if (Blocked)
                {
                    throw new AccessDenied("blocked");
                }
                _telegramId = value; // Устанавливаем значение в закрытое поле

                AddDomainEvent(
                    new UserUpdated(
                        user: this,
                        fieldName: "TelegramId"
                    )
                );
            }
        }
        public bool Blocked { get; set; } = false;
        public Countries Location { get; set; }

        public static UserEntity Create(
            string fullname,
            string email,
            string password,
            PasswordService passwordService)
        {
            var user = new UserEntity()
            {
                Email = email,
                Fullname = fullname,
            };
            var hashedPassword = passwordService.HashPassword(user, password);

            user.HashedPassword = hashedPassword;

            user.AddDomainEvent(new UserCreated(User: user));

            return user;
        }

        public void Block()
        {
            Blocked = true;

            AddDomainEvent(new UserBlocked(this));
        }

        public void UpdatePassword(string oldPassword, string newPassword, PasswordService passwordService)
        {
            var result = passwordService.VerifyHashedPassword(
                this, HashedPassword, oldPassword);
            if (result != PasswordVerificationResult.Success)
            {
                throw new AccessDenied("Wrong password");
            }

            HashedPassword = passwordService.HashPassword(this, newPassword);
        }

        public void UpdatePassword(string newPassword, PasswordService passwordService)
        {
            HashedPassword = passwordService.HashPassword(this, newPassword);
        }

        public void Warn(string reason, UserEntity byUser)
        {
            if (Blocked)
            {
                return;
            }
            var warn = new WarningEntity() { Reason = reason, ByUserId = byUser.Id };

            Warnings.Add(warn);
            if (Warnings.Count > 3)
            {
                Block();
                return;
            }
            AddDomainEvent(new UserWarned(this, reason));
        }

        public void SetRole(UserRoles role)
        {
            Role = role;

            AddDomainEvent(new UserRoleUpdated(this, role)); 
        }
    }
}
