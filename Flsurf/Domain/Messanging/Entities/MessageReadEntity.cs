using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class MessageReadEntity : BaseAuditableEntity
    {
        [ForeignKey("ReadBy")]
        public Guid ReadById { get; private set; }
        public UserEntity ReadBy { get; private set; } = null!;
        [ForeignKey(nameof(ChatEntity))]
        public Guid ChatId { get; private set; }
        public DateTime ReadAt { get; private set; }

        public static MessageReadEntity Create(Guid chatId, Guid readById, DateTime readAt)
        {
            return new MessageReadEntity()
            {
                ChatId = chatId,
                ReadById = readById,
                ReadAt = readAt,
            };
        }
    }
}
