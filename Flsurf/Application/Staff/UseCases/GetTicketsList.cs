using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetTicketsList : BaseUseCase<GetTicketsDto, List<TicketEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetTicketsList(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<List<TicketEntity>> Execute(GetTicketsDto dto)
        {
            IQueryable<TicketEntity> query = _context.Tickets;
            var currentUser = await _permService.GetCurrentUser();
            var perm = ZedStaffUser.WithId(currentUser.Id).CanReadTicket(ZedTicket.WithWildcard());

            if (dto.UserId != null)
            {
                query = query.Where(x => x.CreatedBy.Id == dto.UserId);
            }

            if (dto.SubjectId != null)
            {
                await _permService.EnforceCheckPermission(perm);
                query = query.Where(x => x.Subject.Id == dto.SubjectId);
            }

            if (dto.IsAssignedToMe == true)
            {
                await _permService.EnforceCheckPermission(perm);
                query = query.Where(x => x.AssignedUser.Id == currentUser.Id);
            }

            var tickets = await query.ToListAsync();

            return tickets;
        }
    }
}
