using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class TaskEntity : BaseAuditableEntity
    {
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; } = null!;
        public string TaskTitle { get; set; } = null!;
        public string TaskDescription { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Priority { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; } = null!;  
    }
}
