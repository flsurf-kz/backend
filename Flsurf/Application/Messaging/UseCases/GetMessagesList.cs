using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetMessagesList(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<GetMessagesListDto, ICollection<MessageEntity>>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService; 

        public async Task<ICollection<MessageEntity>> Execute(GetMessagesListDto dto)
        {
            var currentUser = await _permissionService.GetCurrentUser();
            var chat = await _dbContext.Chats.FirstOrDefaultAsync(x => x.Id == dto.ChatId); 

            Guard.Against.NotFound(dto.ChatId, chat);

            await _permissionService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanReadChat(ZedChat.WithId(dto.ChatId))); 

            var messages = await _dbContext.Messages
                .Where(x => x.ChatId == dto.ChatId)
                .Where(x => x.IsDeleted == false)
                .IncludeStandard()
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(dto.Starts, dto.Ends)
                .ToListAsync();

            return messages;
        }
    } 
}
