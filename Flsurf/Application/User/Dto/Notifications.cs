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

    public class NotificationDto
    {
        /// <summary>
        /// Идентификатор уведомления
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Заголовок уведомления
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Текст уведомления
        /// </summary>
        [Required]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Кем отправлено (может быть null для системных уведомлений)
        /// </summary>
        public Guid? FromUserId { get; set; }

        /// <summary>
        /// Кому отправлено
        /// </summary>
        [Required]
        public Guid ToUserId { get; set; }

        /// <summary>
        /// Тип уведомления
        /// </summary>
        [Required]
        public NotificationTypes Type { get; set; }

        /// <summary>
        /// Дополнительные данные (в JSON-строке)
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Признак скрытого уведомления
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Иконка уведомления (Id файла), если задана
        /// </summary>
        public Guid? IconId { get; set; }

        /// <summary>
        /// Временная метка создания (из BaseAuditableEntity)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Временная метка последнего изменения (из BaseAuditableEntity)
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }
    }
}
