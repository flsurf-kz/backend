using Flsurf.Application.Staff.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Staff.Perms
{
    // Spec: read, create, update, delete 
    public class ZedStaffUser : ResourceReference
    {
        // copy of the User/Permissions/ZedUser but for staff bounded context
        // 
        // reason behind this decision is isolation between modules, and not relevant permissions in User/ZedUser
        private ZedStaffUser(Guid userId) : base($"flsurf/user:{userId}") { }

        public static ZedStaffUser WithId(Guid userId) => new(userId);

        public Permission CanCloseTicket(ZedTicket ticket) => new(this, "close", ticket);

        public Permission CanReadTicket(ZedTicket ticket) => new(this, "read", ticket);

        public Permission CanUpdateTicket(ZedTicket ticket) => new(this, "update", ticket);

        public Permission CanAddComment(ZedTicket ticket) => new(this, "add_comment", ticket);
        public Permission CanCreateNews() => new(this, "create", ZedNews.WithWildcard());
        public Permission CanUpdateNews() => new(this, "update", ZedNews.WithWildcard());
        public Permission CanDeleteNews() => new(this, "delete", ZedNews.WithWildcard());

    }
}
