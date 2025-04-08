using Flsurf.Domain.Common;
using Flsurf.Domain.Staff.Entities;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketCommentAdded(TicketCommentEntity comment, TicketEntity ticket) : DomainEvent
    {
        public Guid TicketId { get; } = ticket.Id; 
        public Guid CommentId { get; } = comment.Id;
    }

    public class TicketCommentRemoved(TicketCommentEntity comment, TicketEntity ticket) : DomainEvent
    {
        public Guid TicketId { get; } = comment.Id;
        public Guid CommentId { get; } = ticket.Id; 
    }
}
