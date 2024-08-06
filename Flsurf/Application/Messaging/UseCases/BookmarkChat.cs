using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Messaging.UseCases
{
    public class BookmarkChat : BaseUseCase<BookmarkChatDTO, bool>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public BookmarkChat(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<bool> Execute(BookmarkChatDTO dto)
        {

            return true;
        }
    }
}
