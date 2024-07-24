using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Presentation.Web.Services;
using Microsoft.EntityFrameworkCore;
using SpiceDb;
using SpiceDb.Models;
using System.Security.AccessControl;

namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public class SpiceDbPermService : UserPolicy, IPermissionService
    {
        private ISpiceDbClient _client;

        public SpiceDbPermService(
            ISpiceDbClient client, 
            IApplicationDbContext dbContext, 
            IUser currentUser
        ) : base(dbContext, currentUser)
        {
            _client = client;
        }

        public async Task<bool> CheckPermission(string resource, string relation, string subject)
        {
            return await CheckPermission(new Permission(resource, relation, subject));
        }

        public async Task<bool> CheckPermission(Permission perm)
        {
            return (await _client.CheckPermissionAsync(perm)).HasPermission; 
        }

        public async Task<bool> AddRelationship(Relationship rel)
        {
            await _client.AddRelationshipAsync(rel);
            // zedtoken is not used, because this is monolith,
            // and we dont need timed constitency because we have consistent database all across application
            return true; 
        }
        public async Task<bool> AddRelationship(string resource, string relation, string subject)
        {
            return await AddRelationship(new Relationship(resource, relation, subject));
        }
        public async Task<bool> DeleteRelationship(Relationship rel)
        {
            await _client.DeleteRelationshipAsync(rel);
            return true; 
        }
        public async Task<bool> DeleteRelationship(string resource, string relation, string subject)
        {
            return await DeleteRelationship(new Relationship(resource, relation, subject));
        }
        public async Task<List<string>> GetResourcePermissions(string resourceType, string relation, ResourceReference subject)
        {
            return await _client.GetResourcePermissionsAsync(resourceType, relation, subject); 
        }

        public async Task<bool> EnforceCheckPermission(Permission perm)
        {
            if (!await CheckPermission(perm))
            {
                throw new AccessDenied($"permission: {perm}"); 
            }
            return true; 
        }

        public async Task<bool> EnforceCheckPermission(string resource, string relation, string subject)
        {
            if (!await CheckPermission(resource, relation, subject))
            {
                throw new AccessDenied($"permission: {resource}#{relation}:{subject}"); 
            }
            return true; 
        }
    }
}
