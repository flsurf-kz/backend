using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedFreelanceUser : ResourceReference
    {
        private ZedFreelanceUser(Guid userId) : base($"freelance/user:{userId}") { }

        public static ZedFreelanceUser WithId(Guid userId) => new(userId);

        public Permission CanReadContract(ZedContract contract) => new(contract, "read", this);

        public Permission CanReadContracts(ZedContract[] contracts) =>
            new(contracts, "read", this);
    }
}
