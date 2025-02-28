using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class BookmarkedJobEntity : BaseAuditableEntity
    {
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = null!;
        [ForeignKey("User")]
        public Guid UserId { get; set; }
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
