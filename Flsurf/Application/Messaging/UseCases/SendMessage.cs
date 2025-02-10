using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Messaging.UseCases
{
    public class SendMessage(IApplicationDbContext dbContext, IPermissionService permService, UploadFiles uploadFiles) : BaseUseCase<SendMessageDto, MessageEntity>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permService = permService;
        private UploadFiles _uploadFiles = uploadFiles; 

        public async Task<MessageEntity> Execute(SendMessageDto dto)
        {
            var currentUser = await _permService.GetCurrentUser();

            await _permService.EnforceCheckPermission(
                ZedMessangerUser.WithId(currentUser.Id).CanSendMessage(ZedChat.WithId(dto.ChatId))); 

            if (string.IsNullOrEmpty(dto.Text) || dto.Files != null)
            {
                throw new Exception("Неправильный формат"); 
            }

            ICollection<FileEntity> files = []; 

            if (dto.Files != null)
            {
                files = await _uploadFiles.Execute(dto.Files);
            }
            var message = MessageEntity.Create(dto.Text, currentUser, files);

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync(); 

            return message;
        }
    }
}
