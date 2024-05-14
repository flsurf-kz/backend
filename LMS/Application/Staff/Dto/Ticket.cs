﻿using LMS.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Staff.Dto
{
    public class CreateTicketDto
    {
        [Required]
        public Guid SubjectId { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public ICollection<CreateFileDto> Files { get; set; } = [];
    }
    public class GetTicketsDto
    {
        public Guid? UserId { get; set; }
        public Guid? SubjectId { get; set; } = null!;
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
