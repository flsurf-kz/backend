using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class TaskEntity : BaseAuditableEntity
    {
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
