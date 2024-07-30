using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class BookmarkedJobEntity : BaseAuditableEntity
    {
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public JobEntity Job { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
