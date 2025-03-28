using Flsurf.Domain.Freelance.Entities;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedFreelancerUser : ResourceReference
    {
        private ZedFreelancerUser(Guid userId) : base($"flsurf/user:{userId}") { }

        public static ZedFreelancerUser WithId(Guid userId) => new(userId);

        public Permission CanReadContract(ZedContract contract) => new(contract, "read", this);

        public Permission CanReadContracts() =>
            new(ZedContract.WithWildCard(), "read", this);

        public Permission CanDeleteCategory(ZedCategory category) => new(category, "delete", this);
        public Permission CanUpdateCategory(ZedCategory category) => new(category, "update", this);
        public Permission CanInviteMembers(ZedFreelancerTeam team) => new(team, "invite", this);
        public Permission CanKickMembers(ZedFreelancerTeam team) => new(team, "kick", this);
        public Permission CanAddGlobalSkills() => new(ZedSkill.WithWildcard(), "add", this);
        public Permission CanWorkOnContract(ZedContract contract) => new(contract, "work", this);
        public Permission CanCreateCategories() => new(ZedCategory.WithWildcard(), "create", this);
        public Permission CanDeleteJob(ZedJob job) => new(job, "delete", this);
        public Permission CanUpdateJob(ZedJob job) => new(job, "update", this);
        public Permission CanSuspendClientProfile(ZedClientProfile profile) => new(profile, "suspend", this);
        public Permission CanCancelContract(ZedContract contract) => new(contract, "cancel", this);
        public Permission CanApproveContests() => new(ZedContest.WithWildcard(), "approve", this);
        public Permission CanUpdateFreelancerTeam(ZedFreelancerTeam team) => new(team, "update", this); 
    }
}
