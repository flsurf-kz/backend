using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateChat(IApplicationDbContext context, IPermissionService permService) : BaseUseCase<UpdateChatDto, bool>
    {
        private readonly IPermissionService _permissionService = permService;
        private readonly IApplicationDbContext _context = context;

        public async Task<bool> Execute(UpdateChatDto dto)
        {
            var user = await _permissionService.GetCurrentUser();

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(user.Id).CanUpdateChat(ZedChat.WithId(dto.ChatId)));

            var chat = await _context.Chats.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.ChatId);
            Guard.Against.NotFound(dto.ChatId, chat);

            chat.UpdateChat(dto.Name, dto.IsTextingAllowed, dto.IsArchived);

            await _context.SaveChangesAsync();

            return true;
        }
    }

}
