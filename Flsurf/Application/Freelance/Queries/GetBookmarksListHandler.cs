using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetBookmarksListHandler(IPermissionService permService, IApplicationDbContext dbContext
        ) : IQueryHandler<GetBookmarksListQuery, List<BookmarkedJobEntity>>
    {
        private IPermissionService permissionService = permService; 
        private IApplicationDbContext _dbContext = dbContext; 

        public async Task<List<BookmarkedJobEntity>> Handle(GetBookmarksListQuery query)
        {
            // no need for perm logic, bc no need for moderators to see bookmarked jobs they are useless
            var userId = (await permissionService.GetCurrentUser()).Id; 

            var bookmarkedJobs = await _dbContext.BookmarkedJobs.Where(x => x.UserId == userId)
                .Paginate(query.Start, query.Ends)
                .ToListAsync();

            return bookmarkedJobs; 
        }
    }
}
