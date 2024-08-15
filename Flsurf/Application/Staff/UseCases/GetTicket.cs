using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetTicket : BaseUseCase<GetTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetTicket(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<TicketEntity> Execute(GetTicketDto dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            Guard.Against.Null(dto.TicketId, nameof(dto.TicketId));

            var currentUser = await _permService.GetCurrentUser();

            // Ensure only the creator of the ticket or higher role can access the ticket
            var ticket = await _context.Tickets
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);
            Guard.Against.Null(ticket, $"Ticket with ID {dto.TicketId} not found.");

            var perm = ZedStaffUser.WithId(currentUser.Id).CanReadTicket(ZedTicket.WithId(ticket.Id));
            await _permService.EnforceCheckPermission(perm);

            return ticket;
        }
    }
}
