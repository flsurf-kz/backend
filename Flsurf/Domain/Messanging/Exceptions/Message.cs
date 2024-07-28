using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Exceptions
{
    public class MessageEntity : BaseAuditableEntity
    {
        [Required]
        [ForeignKey(nameof(UserEntity))]
        public Guid OwnerId { get; private set; }
        public ICollection<UserEntity> Users { get; private set; } = [];
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public ChatTypes Type { get; private set; }
        [Required]
        public bool IsArchived { get; private set; } = false;
        [Required]
        public bool IsTextingAllowed { get; private set; } = true; 
        public DateTime? FinishedAt { get; private set; }
        public ICollection<ContractEntity> Contracts { get; private set; } = []; 
    }
}
