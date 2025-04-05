using Flsurf.Application.Common.Models;
using Flsurf.Application.Files.Dto;
using Flsurf.Domain.User.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Dto
{
    public class GetNotificationsDto : InputPagination
    {
        public Guid? UserId { get; set; }
        public UserRoles? Role { get; set; }
    }


    public class NotificationCreatedDto
    {
        [Required]
        public ICollection<Guid> UserIds { get; set; } = [];
        [Required]
        public int SentToUsers { get; set; } = 0;
    }
}
