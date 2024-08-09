using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class CloseChat : BaseUseCase<CloseChatDto, bool>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public CloseChat(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<bool> Execute(CloseChatDto dto)
        {
            var owner = await _permService.GetCurrentUser(); 
            await _permService.EnforceCheckPermission(
                ZedMessangerUser
                    .WithId(owner.Id)
                    .CanCloseChat(ZedChat.WithId(dto.ChatId)));

            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.Id == dto.ChatId);

            Guard.Against.NotFound(dto.ChatId, chat); 

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync(); 

            return true;
        }
    }
}
