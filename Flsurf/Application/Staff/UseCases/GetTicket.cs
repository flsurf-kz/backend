using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetTicket(IApplicationDbContext dbContext, IPermissionService perm) : BaseUseCase<GetTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context = dbContext;
        private readonly IPermissionService _permService = perm;

        public async Task<TicketEntity> Execute(GetTicketDto dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            Guard.Against.Null(dto.TicketId, nameof(dto.TicketId));

            var currentUser = await _permService.GetCurrentUser();

            // Ensure only the creator of the ticket or higher role can access the ticket
            var ticket = await _context.Tickets
                .Include(x => x.Comments)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);
            Guard.Against.Null(ticket, $"Ticket with ID {dto.TicketId} not found.");


            //if (ticket.CreatedBy.Id != currentUser.Id && !await _accessPolicy.IsAllowed(PermissionEnum.read, ticket))
            //{
            //    throw new AccessDenied("You are not authorized to access this ticket.");
            //}

            return ticket;
        }
    }
}
