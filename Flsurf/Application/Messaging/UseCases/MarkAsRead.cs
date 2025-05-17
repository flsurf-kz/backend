using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class MarkAsRead(IApplicationDbContext dbContext, IPermissionService permissionService)
        : BaseUseCase<MarkAsReadDto, bool>
    {
        private readonly IApplicationDbContext _context = dbContext;
        private readonly IPermissionService _permService = permissionService;

        public async Task<bool> Execute(MarkAsReadDto dto)
        {
            var currentUser = await _permService.GetCurrentUser();
            if (currentUser == null)
            {
                // Handle case where user is not found or not authenticated appropriately
                // For example, throw new UnauthorizedAccessException("User not authenticated.");
                return false; // Or throw an exception
            }

            // Optional: Verify the user is a participant of the chat before marking as read
            bool isParticipant = await _context.Chats
                .AnyAsync(c => c.Id == dto.ChatId &&
                               (c.OwnerId == currentUser.Id || c.Participants.Any(p => p.Id == currentUser.Id)));

            if (!isParticipant)
            {
                // Handle case where user is not part of the chat
                // For example, throw new AccessDeniedException("User is not a participant of this chat.");
                return false; // Or throw
            }

            // The ReadAt timestamp should ideally be the CreatedAt of the latest message in the chat,
            // or DateTime.UtcNow if the chat is empty or to signify "read up to this moment".
            var latestMessageCreatedAt = await _context.Messages
                .Where(m => m.ChatId == dto.ChatId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefaultAsync();

            // Use latest message time, or current time if no messages or if preferred
            var readAtTimestamp = latestMessageCreatedAt ?? DateTime.UtcNow;

            var newReadRecord = MessageReadEntity.Create(dto.ChatId, currentUser.Id, readAtTimestamp);
            _context.MessageReads.Add(newReadRecord);

            await _context.SaveChangesAsync(CancellationToken.None); // Or pass appropriate CancellationToken
            return true;
        }
    }
}
