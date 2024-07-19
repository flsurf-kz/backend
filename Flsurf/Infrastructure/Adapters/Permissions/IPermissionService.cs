namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public interface IPermissionService
    {
        public Task<bool> Check(string user, string relation, string _object);
        public Task<bool> AddRelationship(string fullperm);
        public Task<bool> AddRelationship(string user, string relation, string _object); 

    }
}
