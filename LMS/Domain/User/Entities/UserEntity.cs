﻿using LMS.Domain.Files.Entities;
using LMS.Domain.User.Enums;
using LMS.Domain.User.Events;
using LMS.Domain.User.Interfaces;
using LMS.Domain.User.ValueObjects;
using LMS.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace LMS.Domain.User.Entities
{
    public class UserEntity : BaseAuditableEntity, IAccessUser
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
        [Required]
        public string HashedPassword { get; set; } = null!;
        [Required]
        [EmailAddress(ErrorMessage = "Email address is not correct")]
        public string Email { get; set; } = null!;
        [Required]
        public ICollection<RoleEntity> Roles { get; set; } = [];
        [Column(name: "TelegramId")]
        private string? _telegramId; // Закрытое поле для хранения значения
        public FileEntity? Image { get; set; }
        [Required]
        public bool IsOnline { get; set; } = false;
        [JsonIgnore]
        public ICollection<WarningEntity> Warnings { get; set; } = [];
        public bool IsSuperadmin { get; set; } = false;
        public PermissionCollection Permissions { get; set; } = [];
        public ICollection<GroupEntity> Groups { get; set; } = [];
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

        public ICollection<IRoleEntity> GetRoles()
        {
            return (ICollection<IRoleEntity>)Roles; 
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

        public void AddRole(RoleEntity role)
        {
            if (Roles.Select(x => x.Role).Any(x => x == role.Role))
            {
                return; 
            }
            Roles.Add(role);

            AddDomainEvent(
                new UserUpdated(
                    user: this,
                    fieldName: "Role"
                )
             );
        }

        public void AddGroup(GroupEntity group)
        {
            Groups.Add(group);

            AddDomainEvent(new GroupUserAdded(group, this));
        }

        public ICollection<IPermissionEntity> GetPermissions()
        {
            // is it cachable? intersting...
            var permissions = new List<IPermissionEntity>();
            if (Blocked)
            {
                return []; 
            }
            permissions.AddRange(Permissions);
            foreach (var group in Groups) {
                permissions.AddRange(group.Permissions); 
            }
            return permissions;
        }
    }
}
