using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Flsurf.Domain.Messanging.Entities
{
    public class MessageEntity : BaseAuditableEntity
    {
        [ForeignKey("Sender")]
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; set; } = null!;
        public string Text { get; set; } = null!;
        public bool IsDeleted { get; set; } = false; 
        [ForeignKey("Chat")]
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public bool IsPinned { get; set; } = false; 
        public Guid? ReplyedToMessageId { get; set; } 
        public ICollection<FileEntity> Files { get; set; } = []; 

        public static MessageEntity Create(string text, UserEntity sender, ICollection<FileEntity> files, Guid chatId)
        {
            var message = new MessageEntity { Text = text, SenderId = sender.Id, Sender = sender, Files = files, ChatId = chatId };
            message.AddDomainEvent(new MessageCreated(message.Id));
            return message;
        }

        public static MessageEntity Reply(string text, UserEntity sender, Guid replyedToMsgId, ICollection<FileEntity> files)
        {
            var message = new MessageEntity { 
                Text = text,
                SenderId = sender.Id,
                Sender = sender,
                Files = files, 
                ReplyedToMessageId = replyedToMsgId };
            message.AddDomainEvent(new MessageCreated(message.Id));
            return message;
        }

        public void PinOrUnpin()
        {
            IsPinned = !IsPinned;
            AddDomainEvent(new MessagePinned(this.Id));
        }
    }
}
