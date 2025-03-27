using Flsurf.Application.Common.Models;
using Flsurf.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Staff.Dto
{
    public class CreateTicketDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty; 
        [Required]
        public string Text { get; set; } = string.Empty;
        public ICollection<CreateFileDto> Files { get; set; } = [];
        public double PriorityScore { get; set; }
        public Guid? LinkedDisputeId { get; set; }
        public string Title { get; set; } = string.Empty; 
    }
    public class GetTicketsDto : InputPagination
    {
        public Guid? UserId { get; set; }
        public string? Subject { get; set; } = string.Empty;
        public bool? IsAssignedToMe { get; set; }
    }

    public class UpdateTicketDto
    {
        public Guid TicketId { get; set; }
        public string? Subject { get; set; }
        public string? Text { get; set; }
        public ICollection<CreateFileDto>? Files { get; set; }
        public Guid? AssignUserId { get; set; }
    }

    public class GetTicketDto
    {
        public Guid TicketId { get; set; }
    }
}
