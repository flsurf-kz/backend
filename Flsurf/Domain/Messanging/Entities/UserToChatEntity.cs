using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Domain.Messanging.Entities
{
    public class UserToChatEntity 
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }

        public UserEntity User { get; set; } = null!;
        public ChatEntity Chat { get; set; } = null!;

        public bool NotificationsDisabled { get; set; } = false; 
        public bool Bookmarked { get; private set; } = false; 

        public void Bookmark()
        {
            Bookmarked = true; 
        }
    }
}
