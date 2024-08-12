using Flsurf.Application.Messaging.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Files.Permissions
{
    public class ZedFileUser : ResourceReference
    {
        private ZedFileUser(Guid userId) : base($"flsurf/user:{userId}") { }

        public static ZedFileUser WithId(Guid userId) => new(userId);

        public Permission CanCreateFiles() => new(this, "create", ZedFile.WithWildcard());

        public Permission CanDeleteFile(ZedFile file) => new(this, "delete", file);
    }
}
