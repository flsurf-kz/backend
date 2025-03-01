using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class MemberInvited(UserEntity user) : BaseEvent
    {
        public UserEntity User { get; set; } = user;
    }

    public class InvitationReacted(UserEntity user, bool accepted) : BaseEvent
    {
        public UserEntity User { get; set; } = user;
        public bool Accepted { get; set; } = accepted;
    }

    public class MemberKicked(UserEntity user) : BaseEvent
    {
        public UserEntity User { get; set; } = user;
    }

    public class TeamDisbanded(FreelancerTeamEntity team) : BaseEvent
    {
        public FreelancerTeamEntity Team { get; set; } = team;
    }

    public class MemberRoleChanged(UserEntity user, string newRole) : BaseEvent
    {
        public UserEntity User { get; set; } = user;
        public string NewRole { get; set; } = newRole;
    }

    public class TeamProjectAssigned(FreelancerTeamEntity team, ContractEntity project) : BaseEvent
    {
        public FreelancerTeamEntity Team { get; set; } = team;
        public ContractEntity Project { get; set; } = project;
    }

}
