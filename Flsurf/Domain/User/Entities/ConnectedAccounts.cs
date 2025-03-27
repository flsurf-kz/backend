using Flsurf.Domain.User.Enums;

namespace Flsurf.Domain.User.Entities
{
    public class ConnectedAccountEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty; 
        public Guid Avatar { get; set; }
        public string Link { get; set; } = string.Empty;
        public ConnectedAccountTypes Type { get; set; }
    }
}
