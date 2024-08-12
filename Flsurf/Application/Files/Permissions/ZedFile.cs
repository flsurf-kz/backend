using Flsurf.Application.Messaging.Permissions;
using Flsurf.Application.User.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Files.Permissions
{
    public class ZedFile : ResourceReference
    {
        private ZedFile(string fileId) : base($"flsurf/file:{fileId}") { }

        public static ZedFile WithId(Guid fileId) => new(fileId.ToString());
        public static ZedFile WithWildcard() => new("*");
    }
}
