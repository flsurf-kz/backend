using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;

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
            _permService.EnforceCheckPermission(
                ZedMessangerUser.WithId(owner.Id).CanUpdateMessage(ZedMessage.WithId(dto.MessageId))); 

            return true;
        }
    }
}
