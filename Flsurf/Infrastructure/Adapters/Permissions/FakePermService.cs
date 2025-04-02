using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using SpiceDb.Models;
using System.Collections.Generic;

namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public static class AsyncEnumerableHelper
    {
        public static async IAsyncEnumerable<T> Empty<T>()
        {
            yield break;
        }
    }

    public class FakePermissionService : UserPolicy, IPermissionService
    {
        public FakePermissionService(IApplicationDbContext dbContext,
            IUser currentUser) : base(dbContext, currentUser) { } 

        public Task<bool> CheckPermission(string resource, string relation, string subject)
        {
            return Task.FromResult(true); // Всегда разрешено
        }

        public Task<bool> CheckPermission(Permission perm)
        {
            return Task.FromResult(true); // Всегда разрешено
        }

        public Task<bool> EnforceCheckPermission(Permission perm)
        {
            return Task.FromResult(true); // Всегда разрешено
        }

        public Task<bool> EnforceCheckPermission(string resource, string relation, string subject)
        {
            return Task.FromResult(true); // Всегда разрешено
        }

        public Task<bool> AddRelationship(Relationship fullperm)
        {
            return Task.FromResult(true);
        }

        public Task<bool> AddRelationship(string resource, string relation, string subject)
        {
            return Task.FromResult(true);
        }

        public Task<bool> AddRelationships(params Relationship[] relationships)
        {
            return Task.FromResult(true);
        }

        public Task<bool> AddRelationships(List<Relationship> relationships)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRelationship(Relationship fullperm)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRelationship(string resource, string relation, string subject)
        {
            return Task.FromResult(true);
        }

        public Task<List<string>> GetResourcePermissions(string resource, string relation, ResourceReference subject)
        {
            return Task.FromResult(new List<string> { "read", "write" });
        }

        public IAsyncEnumerable<LookupSubjectsResponse> LookupSubjects(ResourceReference resource, string relation, string subjectType)
        {
            return AsyncEnumerableHelper.Empty<LookupSubjectsResponse>();
        }
    }
}
