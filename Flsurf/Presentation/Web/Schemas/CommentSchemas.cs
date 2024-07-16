using Flsurf.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Presentation.Web.Schemas
{
    public class CreateCommentScheme
    {
        [Required]
        public string Text { get; set; } = null!;
        public Guid? ParentCommentId { get; set; }
        public ICollection<CreateFileDto> Files { get; set; } = [];
    }
}
