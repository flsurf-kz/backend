using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class InviteMember(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<InviteMemberDto, ChatInvitationEntity>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService;

        public async Task<ChatInvitationEntity> Execute(InviteMemberDto dto)
        {
            var user = await _dbContext.Users.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.UserId);

            Guard.Against.NotFound(dto.UserId, user);

            var chat = await _dbContext.Chats.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.ChatId);

            Guard.Against.NotFound(dto.ChatId, chat);

            var currentUser = await _permissionService.GetCurrentUser(); 

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanInviteChatMembers(ZedChat.WithId(chat.Id)));

            var invitation = ChatInvitationEntity.Create("хуй", chat, currentUser, user);

            _dbContext.Invitations.Add(invitation);

            await _dbContext.SaveChangesAsync(); 

            return invitation;
        }
    }
}
