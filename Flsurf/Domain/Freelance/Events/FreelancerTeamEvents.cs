using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Freelance.Events
{
    public class MemberInvited(UserEntity user) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
    }

    public class InvitationReacted(UserEntity user, bool accepted) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
        public bool Accepted { get; } = accepted;
    }

    public class MemberKicked(UserEntity user) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
    }

    public class TeamDisbanded(FreelancerTeamEntity team) : BaseEvent
    {
        public Guid TeamId { get; } = team.Id;
    }

    public class MemberRoleChanged(UserEntity user, string newRole) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
        public string NewRole { get; } = newRole;
    }

    public class TeamProjectAssigned(FreelancerTeamEntity team, ContractEntity project) : BaseEvent
    {
        public Guid TeamId { get; } = team.Id;
        public Guid ProjectId { get; } = project.Id;
    }

}
