using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class BookmarkChat : BaseUseCase<BookmarkChatDTO, bool>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public BookmarkChat(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<bool> Execute(BookmarkChatDTO dto)
        {
            var user = await _permService.GetCurrentUser();
            var userToChat = await _context.UserToChats.FirstOrDefaultAsync(x => x.ChatId == dto.ChatId && x.UserId == user.Id);

            Guard.Against.NotFound(user.Id, userToChat);

            userToChat.Bookmark();
            await _context.SaveChangesAsync(); 

            return true;
        }
    }
}
