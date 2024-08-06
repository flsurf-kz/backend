using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class ChatEntity : BaseAuditableEntity
    {
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public UserEntity Owner { get; set; } = null!;
        public ICollection<UserEntity> Participants { get; set; } = [];
        public string Name { get; set; } = null!; 
        public ChatTypes Type { get; set; }
        public bool IsArchived { get; set; }
        public bool IsTextingAllowed { get; set; }
        public DateTime? FinishedAt { get; set; }
        public ICollection<ContractEntity> Contracts { get; set; } = []; 
    }
}
