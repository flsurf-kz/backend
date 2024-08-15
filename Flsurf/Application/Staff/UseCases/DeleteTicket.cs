using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class DeleteTicket : BaseUseCase<Guid, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeleteTicket(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<TicketEntity> Execute(Guid ticketId)
        {
            Guard.Against.Null(ticketId, nameof(ticketId));

            var owner = await _permService.GetCurrentUser(); 

            // Perform access check for admin role
            await _permService.EnforceCheckPermission(ZedStaffUser.WithId(owner.Id).CanCloseTicket(ZedTicket.WithId(ticketId))); 

            var ticket = await _context.Tickets
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            Guard.Against.Null(ticket, $"Ticket with ID {ticketId} does not exist.");

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return ticket; // Return the deleted ticket
        }
    }
}
