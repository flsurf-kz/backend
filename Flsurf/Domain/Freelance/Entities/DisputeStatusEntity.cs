using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Domain.Freelance.Entities
{
    public class DisputeStatusHistory : BaseEntity
    {
        public Guid DisputeId { get; set; }
        public DisputeStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
