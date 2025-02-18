using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.Messanging.Exceptions;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class ChatEntity : BaseAuditableEntity
    {
        public Guid OwnerId { get; private set; }
        [ForeignKey("OwnerId")]
        public UserEntity Owner { get; private set; } = null!;
        public ICollection<UserEntity> Participants { get; set; } = [];
        public string Name { get; set; } = null!; 
        public ChatTypes Type { get; set; }
        public bool IsArchived { get; set; } = false; 
        public bool IsTextingAllowed { get; set; }
        public DateTime? FinishedAt { get; set; }
        public ICollection<ContractEntity> Contracts { get; set; } = [];
        public ICollection<MessageReadEntity> ReadRecords { get; set; } = []; 

        public static ChatEntity Create(string name, UserEntity owner, List<UserEntity> participants, bool isTextingAllowed, ChatTypes type)
        {
            if (isTextingAllowed && type != ChatTypes.Group)
                throw new TextingNotAllowedDisabled(type); 
            var chat = new ChatEntity()
            {
                Name = name, 
                Owner = owner,
                Participants = participants, 
                IsTextingAllowed = isTextingAllowed,
                Type = type, 
            };

            chat.AddDomainEvent(new ChatCreated(chat)); 

            return chat; 
        }

        public void AddParticipant(UserEntity user, ChatInvitationEntity invitation)
        {
            // you can add to support any number of participants 
            if (Type != ChatTypes.Group && Type != ChatTypes.Support)
                throw new ParticipantsNotAllowed(Type);
            if (invitation.ChatId != Id)
                throw new InvitiationIsIncorrect(invitation.Id, invitation.ChatId); 
            Participants.Add(user);

            invitation.React(ChatInvitationStatus.Accepted);

            AddDomainEvent(new ChatAddedParticipant(this)); 
        }

        public void Archive()
        {
            if (IsArchived)
                return;
            IsArchived = true;
            AddDomainEvent(new ChatArchived(this)); 
        }
    }
}
