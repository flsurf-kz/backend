using Flsurf.Application.Files.Dto;
using Flsurf.Domain.User.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Dto
{


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

    public class SecurityAnswerDto
    {
        [Required]
        public string Token { get; set; } = null!; 
    }
}
