using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Presentation.Web.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetClientProfileQuery : BaseQuery {  
        public Guid? UserId { get; set; }
    }
    
    public class GetClientProfileHandler(IApplicationDbContext dbContext, IPermissionService permService) : IQueryHandler<GetClientProfileQuery, ClientProfileEntity?> 
    {
        public async Task<ClientProfileEntity?> Handle(GetClientProfileQuery query)
        {
            var userId = query.UserId;
            if (userId == null)
                userId = (await permService.GetCurrentUser()).Id; 

            return await dbContext.ClientProfiles
                .Include(x => x.Jobs)
                .Include(x => x.User)
                .Include(x => x.Contracts)
                .FirstOrDefaultAsync(); 
        }
    }
}
