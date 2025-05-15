using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.User.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Staff.Entities
{
    public class NewsEntity : BaseAuditableEntity 
    {
        [Required]
        public string Text { get; set; } = null!; 
        [Required, JsonIgnore]
        public UserEntity Author { get; set; } = null!;
        [JsonIgnore]
        public Guid AuthorId { get; set; }
        public ICollection<FileEntity>? Attachments { get; set; } = [];
        [Required]
        public string Title { get; set; } = null!;
        public bool IsHidden { get; set; } = false; 
        public DateTime PublishTime { get; set; }

        public static NewsEntity Create(string title, string text, ICollection<FileEntity> attachments, DateTime publishTime, UserEntity author)
        {
            return new NewsEntity()
            {
                Title = title,
                Text = text,
                Attachments = attachments,
                PublishTime = publishTime,
                IsHidden = publishTime > DateTime.Now,
                Author = author,
                AuthorId = author.Id,
            }; 
        }
    }
}
