using SpiceDb.Models;
using System.Text.RegularExpressions;

namespace Flsurf.Application.User.Permissions
{
    // No can_ prefix in permissions!!! 
    public class ZedGroup : ResourceReference
    {
        private ZedGroup(Guid groupId) : base($"flsurf/group:{groupId}") { }

        public static ZedGroup WithId(Guid groupId) => new ZedGroup(groupId);

        public Relationship Owner(ZedUser user) => new(user, "owner", this);

        public Relationship Member(ZedUser user) => new(user, "participant", this);

        public Permission CanInviteMembers(ZedUser user) => new(user, "invite_members", this);

        // only owners has permission 
        public Permission CanKickMembers(ZedUser user) => new(user, "kick_members", this); 
    }
}
