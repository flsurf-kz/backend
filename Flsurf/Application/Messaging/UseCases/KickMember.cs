using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class KickMember(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<KickMemberDto, bool>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService;

        public async Task<bool> Execute(KickMemberDto dto)
        {
            var user = await _dbContext.Users.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.UserId);
            Guard.Against.NotFound(dto.UserId, user);

            var chat = await _dbContext.Chats.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.ChatId);
            Guard.Against.NotFound(dto.ChatId, chat);

            var currentUser = await _permissionService.GetCurrentUser();

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanKickChatMembers(ZedChat.WithId(chat.Id)));

            if (!chat.Participants.Contains(user))
                throw new InvalidOperationException("User is not a participant of this chat.");

            chat.Participants.Remove(user);

            chat.AddDomainEvent(new ChatRemovedParticipant(chat, user));

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
