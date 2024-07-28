using Flsurf.Application.User.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Messaging.Permissions
{
    public class ZedChat : ResourceReference
    {
        private ZedChat(Guid chatId) : base($"flsurf/chat:{chatId}") { }

        public static ZedChat WithId(Guid chatId) => new(chatId);

        public Relationship Owner(ZedMessangerUser user) => new(user, "owner", this);
        public Relationship Member(ZedMessangerUser user) => new(user, "member", this);
        // Is user U overwatcher of Chat?  
        // used for moderation, for readonly! 
        public Relationship Overwatcher(ZedMessangerUser user) => new(user, "overwatcher", this);

    }
}
