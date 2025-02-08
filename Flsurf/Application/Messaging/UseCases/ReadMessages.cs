using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class ReadMessages(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<Guid, String>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService;


        public async Task<String> Execute(Guid chatId)
        {
            var currentUser = await _permissionService.GetCurrentUser();
            var chat = await _dbContext.Chats.FirstOrDefaultAsync(x => x.Id == chatId);

            Guard.Against.NotFound(chatId, chat);

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanReadChat(ZedChat.WithId(chatId)));

            

            return "";
        }
    }
}
