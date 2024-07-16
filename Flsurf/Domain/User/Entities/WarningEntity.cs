using Flsurf.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.User.Entities
{
    public class WarningEntity : BaseAuditableEntity
    {
        public string Reason { get; set; } = null!;
        [ForeignKey(nameof(UserEntity))]
        public Guid ByUserId { get; set; }
        public bool Expired { get; set; } = false;
        public DateTime? ExpiresIn { get; set; }
    }
}
