using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Entities;
using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerTeamEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserEntity> Participants { get; set; } = [];
        public bool Closed { get; private set; } = false;
        public string? ClosedReason { get; private set; } = null;

        [ForeignKey("Avatar")]
        public Guid AvatarId { get; set; }
        public FileEntity Avatar { get; set; } = null!;

        public UserEntity Owner { get; set; } = null!;
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ICollection<FreelancerTeamInvitation> Invitations { get; set; } = [];
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<ContractEntity> AssignedContracts { get; set; } = []; 

        /// <summary>
        /// Пригласить пользователя в команду
        /// </summary>
        public void InviteMember(UserEntity user)
        {
            if (Participants.Contains(user))
                throw new InvalidOperationException("Пользователь уже в команде.");

            var invitation = FreelancerTeamInvitation.Create(user, this);
            Invitations.Add(invitation);
            AddDomainEvent(new MemberInvited(user));
        }

        public void AddMember(UserEntity user, FreelancerTeamInvitation invitation)
        {
            if (invitation.UserId != user.Id)
            {
                throw new DomainException("Не тот пользватель"); 
            }
            if (!Invitations.Contains(invitation))
            {
                throw new DomainException("Нету приглашения"); 
            }

            if (Participants.Contains(user))
                throw new DomainException("Пользователь уже в команде.");

            Participants.Add(user);
            AddDomainEvent(new InvitationReacted(user, true));
        }

        /// <summary>
        /// Исключить участника из команды
        /// </summary>
        public void RemoveMember(UserEntity user)
        {
            if (!Participants.Contains(user))
                throw new InvalidOperationException("Пользователь не состоит в команде.");

            Participants.Remove(user);
            AddDomainEvent(new MemberKicked(user));
        }

        /// <summary>
        /// Изменить роль участника
        /// </summary>
        public void ChangeMemberRole(UserEntity user, string newRole)
        {
            if (!Participants.Contains(user))
                throw new InvalidOperationException("Пользователь не состоит в команде.");

            AddDomainEvent(new MemberRoleChanged(user, newRole));
        }

        /// <summary>
        /// Закрыть команду
        /// </summary>
        public void CloseTeam(string reason)
        {
            if (Closed)
                throw new InvalidOperationException("Команда уже закрыта.");

            Closed = true;
            ClosedReason = reason;
            AddDomainEvent(new TeamDisbanded(this));
        }

        /// <summary>
        /// Назначить команде проект
        /// </summary>
        public void AssignContract(ContractEntity contrct)
        {
            AssignedContracts.Add(contrct); 
            AddDomainEvent(new TeamProjectAssigned(this, contrct));
        }
    }

}
