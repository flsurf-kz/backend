using Flsurf.Domain.User.Enums;

namespace Flsurf.Domain.User.Entities
{
    public class ConnectedAccountEntity : BaseAuditableEntity
    {
        public string Name { get; set; }
        public Guid Avatar { get; set; }
        public string Link { get; set; }
        public ConnectedAccountTypes  Type { get; set; }
    }
}
