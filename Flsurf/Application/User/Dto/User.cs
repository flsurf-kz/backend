using Flsurf.Application.Files.Dto;
using Flsurf.Domain.User.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Dto
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required] 
        public string Surname { get; set; } = null!;
        public string? Phone { get; set; } 
        public string Password { get; set; } = null!;
        [Required]
        public string EmailAddress { get; set; } = null!;
        [Required]
        public UserTypes UserType { get; set; }
        public UserRoles? Role { get; set; }
    }

    // DTO для внешних пользователей
    public class ExternalUserDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty; 
        public string AvatarUrl { get; set; } = string.Empty;
        [Required]
        public string Provider { get; set; } = string.Empty; // Google, VK и т.д.
    }

    public class GetUserDto
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; } = null!;
    }

    public class GetListUserDto
    {
        public string? Fullname { get; set; }
        public UserRoles? Role { get; set; }
    }

    public class WarnUserDto
    {
        public string Reason { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }

    public class UpdateUserRoleDto
    {
        [Required]
        public UserRoles Role { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }

    public class UpdateUserDto
    {
        [Required]
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public UserRoles? Role { get; set; }
        public CreateFileDto? Avatar { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? TelegramId { get; set; }
        public bool? Blocked { get; set; }
    }

    public class BlockUserDto
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        [EmailAddress(ErrorMessage = "No email")]
        public string Email { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
    }

    public class SendResetCodeDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "No email")]
        public string Email { get; set; } = null!;

    }
}
