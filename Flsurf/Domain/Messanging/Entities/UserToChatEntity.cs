using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Messanging.Events;

namespace Flsurf.Domain.Messanging.Entities
{
    public class UserToChatEntity : BaseAuditableEntity
    {
        [Key, Column(Order = 1)]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey("Chat")]
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!;
        public bool NotificationsDisabled { get; set; } = false; 
        public bool Bookmarked { get; set; } = false; 

        public void Bookmark()
        {
            Bookmarked = true; 

            AddDomainEvent(new ChatBookmarked(this)); 
        }
    }
}
