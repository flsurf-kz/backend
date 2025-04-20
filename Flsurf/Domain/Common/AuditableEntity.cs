using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        public Guid? CreatedById { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        public Guid? LastModifiedById { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
