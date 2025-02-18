using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Messanging.Entities
{
    public class ChatInvitationEntity : BaseAuditableEntity
    {
        public string Text { get; set; } = null!; 
        [ForeignKey("Chat")]
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!;
        public UserEntity InvitedBy { get; set; } = null!; 
        public UserEntity User { get; set; } = null!;
        public ChatInvitationStatus Status { get; set; } = ChatInvitationStatus.Waiting;

        public static ChatInvitationEntity Create(string text, ChatEntity chat, UserEntity invitedBy, UserEntity user)
        {
            if (chat is null)
                throw new ArgumentNullException(nameof(chat));
            if (invitedBy is null)
                throw new ArgumentNullException(nameof(invitedBy));
            if (user is null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Invitation text cannot be empty.", nameof(text));

            var invitation = new ChatInvitationEntity
            {
                Text = text,
                Chat = chat,
                ChatId = chat.Id,
                InvitedBy = invitedBy,
                User = user,
                Status = ChatInvitationStatus.Waiting
            };

            invitation.AddDomainEvent(new ChatInvitationCreated(invitation));

            return invitation;
        }

        public void React(ChatInvitationStatus status)
        {
            if (ChatInvitationStatus.Accepted == status)
                return; // Если статус не меняется, ничего не делаем

            Status = status;

            AddDomainEvent(new ChatInvitationUsed(this));
        }
    }
}
