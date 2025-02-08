using SpiceDb.Models;

namespace Flsurf.Application.Staff.Perms
{
    public class ZedComment : ResourceReference
    {
        // copy of the User/Permissions/ZedUser but for staff bounded context
        // 
        // reason behind this decision is isolation between modules, and not relevant permissions in User/ZedUser
        private ZedComment(Guid commentId) : base($"flsurf/comment:{commentId}") { }

        public static ZedComment WithId(Guid commentId) => new(commentId);

        public Relationship Owner(ZedStaffUser user) => new(user, "owner", this); 
    }
}
