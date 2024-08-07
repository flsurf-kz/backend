﻿using Flsurf.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Staff.Dto
{
    public class CreateCommentDto
    {
        [Required]
        public string Text { get; set; } = null!;
        [Required]
        public Guid TicketId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public ICollection<CreateFileDto> Files { get; set; } = [];
    }

    public class GetCommentsDto
    {
        public Guid? UserId { get; set; }
        public Guid? TicketId { get; set; }
    }
}
