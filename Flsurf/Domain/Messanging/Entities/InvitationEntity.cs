using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class InvitationEntity : BaseAuditableEntity
    {
        public string Text { get; set; } = null!; 
        [ForeignKey("Chat")]
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!; 
        public ICollection<UserEntity> InvitedBy { get; set; } = [];
        public UserEntity User { get; set; } = null!;
    }
}
