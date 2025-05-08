using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.Messanging.Exceptions;
using Flsurf.Domain.User.Entities;
using MailKit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public ICollection<MessageReadEntity> ReadRecords { get; set; } = [];

        // DONT LOAD ALL OF THEM, LAST MESSAGE REQUIRED IT 
        [JsonIgnore]
        public ICollection<MessageEntity> Messages { get; set; } = [];

        [NotMapped]
        public MessageEntity? LastMessage => Messages
            .OrderByDescending(m => m.CreatedAt) 
            .FirstOrDefault();

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

            chat.AddDomainEvent(new ChatCreated(chat.Id)); 

            return chat; 
        }

        public void UpdateChat(string? name, bool? isTextingAllowed, bool? isArchived)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (isTextingAllowed.HasValue)
            {
                if (isTextingAllowed.Value && Type != ChatTypes.Group)
                    throw new TextingNotAllowedDisabled(Type);

                IsTextingAllowed = isTextingAllowed.Value;
            }

            if (isArchived.HasValue && isArchived.Value)
                Archive(); // Используем существующий метод Archive()

            AddDomainEvent(new ChatUpdated(this.Id));
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

            AddDomainEvent(new ChatAddedParticipant(this.Id)); 
        }

        public void KickMember(UserEntity user, UserEntity kickedBy)
        {
            if (Type != ChatTypes.Group)
                throw new ParticipantsNotAllowed(Type);

            if (!Participants.Contains(user))
                throw new InvalidOperationException("User is not a participant of this chat.");

            Participants.Remove(user);

            AddDomainEvent(new ChatRemovedParticipant(this.Id, user.Id, kickedBy.Id));
        }


        public void Archive()
        {
            if (IsArchived)
                return;
            IsArchived = true;
            AddDomainEvent(new ChatArchived(this.Id)); 
        }
    }
}
