using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Messanging.Entities
{
    public class UserToChatEntity : BaseAuditableEntity
    {
        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }
        [Key, Column(Order = 2)]
        public Guid ChatId { get; set; }
        [ForeignKey("ChatId")]
        public ChatEntity Chat { get; set; } = null!;

        [ForeignKey("UserId")]
        public UserEntity User { get; set; } = null!; 
        public bool NotificationsDisabled { get; set; } = false; 
        public bool Bookmarked { get; set; } = false; 

        public void Bookmark()
        {
            Bookmarked = true; 

            AddDomainEvent(new ChatBookmarked(this)); 
        }
    }
}
