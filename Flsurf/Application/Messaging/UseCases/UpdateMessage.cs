using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateMessage : BaseUseCase<UpdateMessageDto, bool>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public UpdateMessage(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }
        
        public async Task<bool> Execute(UpdateMessageDto dto)
        {
            var owner = await _permService.GetCurrentUser();
            await _permService.EnforceCheckPermission(
                ZedMessangerUser.WithId(owner.Id).CanUpdateMessage(ZedMessage.WithId(dto.MessageId)));

            var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == dto.MessageId);

            Guard.Against.NotFound(dto.MessageId, message); 

            if (dto.Text != null)
            {
                message.Text = dto.Text; 
            }

            // IGNORE PHOTOS! 

            _context.Messages.Update(message); 
            await _context.SaveChangesAsync(); 

            return true;
        }
    }
}
