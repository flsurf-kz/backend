using SpiceDb.Models;

namespace Flsurf.Application.Messaging.Permissions
{
    public class ZedMessage : ResourceReference
    {
        private ZedMessage(Guid messageId) : base($"flsurf/message:{messageId}") { }

        public static ZedMessage WithId(Guid messageId) => new(messageId);
    }
}
