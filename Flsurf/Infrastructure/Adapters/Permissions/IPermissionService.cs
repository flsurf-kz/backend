using Flsurf.Domain.User.Entities;
using SpiceDb.Models;

namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public interface IPermissionService
    {
        public Task<bool> CheckPermission(string resource, string relation, string subject);
        public Task<bool> CheckPermission(Permission perm); 
        public Task<bool> EnforceCheckPermission(Permission perm);
        public Task<bool> EnforceCheckPermission(string resource, string relation, string subject);
        public Task<bool> AddRelationship(Relationship fullperm);
        public Task<bool> AddRelationships(params Relationship[] relationships);
        public Task<bool> AddRelationships(List<Relationship> relationships);
        public Task<bool> AddRelationship(string resource, string relation, string subject); 
        public Task<bool> DeleteRelationship(Relationship fullperm);
        public Task<bool> DeleteRelationship(string resource, string relation, string subject);
        public Task<List<string>> GetResourcePermissions(string resource, string realtion, ResourceReference subject);
        // Fuck me!! 
        public Task<UserEntity> GetCurrentUser(); 
    }
}
