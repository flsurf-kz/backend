using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Messaging.UseCases
{
    public class CreateChat : BaseUseCase<CreateChatDTO, Guid>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public CreateChat(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<Guid> Execute(CreateChatDTO dto)
        {
            var chat = ChatEntity.Create()

            return;
        }
    }
}
