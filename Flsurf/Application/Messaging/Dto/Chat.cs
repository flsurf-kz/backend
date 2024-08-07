namespace Flsurf.Application.Messaging.Dto
{
    public record BookmarkChatDTO
    {
        public Guid ChatId;
    }

    public record CreateChatDTO {
        public string Name = null!; 
        public string Description = null!;
        public List<Guid> UserIds = []; 
    }
}
