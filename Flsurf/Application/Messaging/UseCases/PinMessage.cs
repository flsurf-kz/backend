using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class PinMessage(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<PinMessageDto, bool>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService;

        public async Task<bool> Execute(PinMessageDto dto)
        {
            var message = await _dbContext.Messages.IncludeStandard().FirstOrDefaultAsync(x => x.Id == dto.MessageId);
            Guard.Against.NotFound(dto.MessageId, message);

            var chat = await _dbContext.Chats.IncludeStandard().FirstOrDefaultAsync(x => x.Id == message.ChatId);
            Guard.Against.NotFound(message.ChatId, chat);

            var currentUser = await _permissionService.GetCurrentUser();

            // Проверяем, может ли текущий пользователь отправлять сообщения (раз это критерий допуска)
            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanSendMessage(ZedChat.WithId(chat.Id)));

            message.PinOrUnpin();

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }

}
