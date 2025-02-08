using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using System.Runtime.InteropServices;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateChat(IApplicationDbContext context, IPermissionService permService) : BaseUseCase<UpdateChatDto, bool>
    {
        private readonly IPermissionService _permissionService = permService;
        private readonly IApplicationDbContext _context = context;

        public async Task<bool> Execute(UpdateChatDto dto)
        {
            var owner = await _permissionService.GetCurrentUser();

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(owner.Id).CanUpdateChat(ZedChat.WithId(dto.ChatId))); 

            return true;
        }
    }
}
