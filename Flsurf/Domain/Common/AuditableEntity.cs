using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        [Required]
        public Guid? CreatedById { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        [Required]
        public Guid? LastModifiedById { get; set; }
        [Required]
        public DateTime? LastModifiedAt { get; set; }
    }
}
