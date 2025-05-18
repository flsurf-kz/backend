using Flsurf.Application.Files.Dto;

namespace Flsurf.Application.Staff.Dto
{
    public class CreateNewsDto
    {
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime PublishTime { get; set; }
        public ICollection<CreateFileDto>? Files { get; set; }
        public bool ChangeNotes { get; set; }
    }

    public class UpdateNewsDto
    {
        public Guid NewsId { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public DateTime? PublishTime { get; set; }
        public bool? IsHidden { get; set; }
        public ICollection<CreateFileDto>? NewFiles { get; set; }
        public bool? ChangeNotes { get; set; }
    }

    public class GetNewsListDto
    {
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 20;
        public bool IncludeHidden { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? ChangeNotes { get; set; }
    }

}
