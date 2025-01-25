using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class DeleteMessage(IPermissionService permService, IApplicationDbContext context) : BaseUseCase<DeleteMessageDto, bool>
    {
        private readonly IPermissionService _permissionService = permService;
        private readonly IApplicationDbContext _applicationDbContext = context;

        public async Task<bool> Execute(DeleteMessageDto dto)
        {

            var byUser = await _permissionService.GetCurrentUser();
            var rel = ZedMessangerUser
                    .WithId(byUser.Id)
                    .CanDeleteMessage(ZedMessage.WithId(dto.MessgeId)); 

            await _permissionService.EnforceCheckPermission(rel); 

            var message = await _applicationDbContext.Messages.FirstOrDefaultAsync(x => x.Id == dto.MessgeId);

            Guard.Against.NotFound(dto.MessgeId, message); 

            await _permissionService.DeleteRelationship(rel);

            _applicationDbContext.Messages.Remove(message);
            await _applicationDbContext.SaveChangesAsync(); 

            return true;
        }
    }
}
