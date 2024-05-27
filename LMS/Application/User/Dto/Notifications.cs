﻿using LMS.Application.Common.Models;
using LMS.Application.Files.Dto;
using LMS.Domain.User.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.User.Dto
{
    public class GetNotificationsDto : InputPagination
    {
        public Guid? UserId { get; set; }
        public UserRoles? Role { get; set; }
    }

    public class CreateNotificationDto
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Text { get; set; } = null!;
        [Required]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
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
