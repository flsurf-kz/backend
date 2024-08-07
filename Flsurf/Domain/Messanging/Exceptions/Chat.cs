using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Messanging.Exceptions
{
    public class TextingNotAllowedDisabled : Exception
    {
        public TextingNotAllowedDisabled(ChatTypes type) 
            : base($"'Text is not allowed' in this type of chat is disabled: {type}") { }
    }

    public class ParticipantsNotAllowed : Exception
    {
        public ParticipantsNotAllowed(ChatTypes type) 
            : base($"Participants is not allowed in this type of chat, type: {type}") { }
    }

    public class InvitiationIsIncorrect : Exception
    {
        public InvitiationIsIncorrect(Guid chatId, Guid invitationId)
            : base($"Invatation is not correct, chatId: {chatId}, invitationId: {invitationId}") { }
    }
}
