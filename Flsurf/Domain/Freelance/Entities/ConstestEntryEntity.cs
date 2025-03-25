using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContestEntryEntity : BaseAuditableEntity
    {
        [ForeignKey("Contest")]
        public Guid ContestId { get; set; }
        public ContestEntity Contest { get; set; } = null!;
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!; 
        public string Description { get; set; } = null!;
        public ContestEntryReaction Reaction { get; set; } = ContestEntryReaction.None;
        public bool Hidden { get; set; }
        public ICollection<FileEntity> Files { get; set; } = []; 
    }
}
