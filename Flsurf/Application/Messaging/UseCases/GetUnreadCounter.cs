using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetUnreadCounterDto // DTO for the use case input
    {
        public Guid ChatId { get; set; }
        // UserId can be passed or fetched from IPermissionService if always for current user
        public Guid UserId { get; set; }
    }

    public class GetUnreadCounter(IApplicationDbContext dbContext) : BaseUseCase<GetUnreadCounterDto, int>
    {
        private readonly IApplicationDbContext _context = dbContext;

        public async Task<int> Execute(GetUnreadCounterDto dto)
        {
            // Find the latest ReadAt timestamp for the user in this chat.
            // Using (DateTime?) allows FirstOrDefaultAsync to return null if no record exists.
            var lastReadTimestamp = await _context.MessageReads
                .Where(mr => mr.ChatId == dto.ChatId && mr.ReadById == dto.UserId)
                .OrderByDescending(mr => mr.ReadAt)
                .Select(mr => (DateTime?)mr.ReadAt)
                .FirstOrDefaultAsync();

            int unreadCount;
            if (lastReadTimestamp.HasValue)
            {
                // Count messages created after the last read timestamp
                unreadCount = await _context.Messages
                    .Where(m => m.ChatId == dto.ChatId && m.CreatedAt > lastReadTimestamp.Value)
                    .CountAsync();
            }
            else
            {
                // If no read record, all messages in the chat are unread for this user
                unreadCount = await _context.Messages
                    .Where(m => m.ChatId == dto.ChatId)
                    .CountAsync();
            }
            return unreadCount;
        }
    }
}
