﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetTicket : BaseUseCase<GetTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;

        public GetTicket(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _context = dbContext;
            _accessPolicy = accessPolicy;
        }

        public async Task<TicketEntity> Execute(GetTicketDto dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            Guard.Against.Null(dto.TicketId, nameof(dto.TicketId));

            var currentUser = await _accessPolicy.GetCurrentUser();

            // Ensure only the creator of the ticket or higher role can access the ticket
            var ticket = await _context.Tickets
                .Include(x => x.Comments)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);
            Guard.Against.Null(ticket, $"Ticket with ID {dto.TicketId} not found.");


            if (ticket.CreatedBy.Id != currentUser.Id && !await _accessPolicy.IsAllowed(PermissionEnum.read, ticket))
            {
                throw new AccessDenied("You are not authorized to access this ticket.");
            }

            return ticket;
        }
    }
}
