using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContestEntryEntity : BaseAuditableEntity
    {
        [ForeignKey("Contest")]
        public Guid ContestId { get; set; }
        public ContestEntity Contest { get; set; }
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; }
        public string EntrySubmission { get; set; }
        public DateTime EntryDate { get; set; }
        public string Status { get; set; }
        public bool Denied { get; set; }
    }
}
