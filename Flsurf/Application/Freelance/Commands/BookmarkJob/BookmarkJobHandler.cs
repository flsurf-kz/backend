using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.BookmarkJob;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Freelance.Commands
{
    // unbookmark is too similar, so unbookmark was merged to this handler 
    public class BookmarkJobHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<BookmarkJobCommand>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(BookmarkJobCommand command)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == command.JobId);

            if (job == null)
            {
                return CommandResult.NotFound("not found", command.JobId);
            }

            var user = await _permissionService.GetCurrentUser();

            var bookmark = BookmarkedJobEntity.Create(job, user);

            _dbContext.BookmarkedJobs.Add(bookmark);

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(bookmark.Id);
        }
    }
}
