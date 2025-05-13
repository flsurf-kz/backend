using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class BookmarkedJobEntity : BaseAuditableEntity
    {
        [ForeignKey("Job"), Required]
        public Guid JobId { get; set; }
        [Required]
        public JobEntity Job { get; set; } = null!;
        [ForeignKey("User"), Required]
        public Guid UserId { get; set; }
        [Required]
        public UserEntity User { get; set; } = null!;
        
        public static BookmarkedJobEntity Create(JobEntity job, UserEntity user)
        {
            var bookmark = new BookmarkedJobEntity
            {
                Job = job,
                User = user,

            };
            bookmark.AddDomainEvent(new JobWasBookmarked(bookmark));

            return bookmark; 
        }
    }
}
