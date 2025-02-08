using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class MessageEntity : BaseAuditableEntity
    {
        [ForeignKey("Sender")]
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; set; } = null!;
        public string Text { get; set; } = null!;
        public Boolean IsDeleted { get; set; } = false; 
        [ForeignKey("Chat")]
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public string Status { get; set; } = null!;
        public ICollection<Guid> Files { get; set; } = []; 
    }
}
