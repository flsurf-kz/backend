using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class UpdateTicket : BaseUseCase<UpdateTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly IFileService _fileService;

        public UpdateTicket(IApplicationDbContext dbContext, IPermissionService permService, IFileService fileService)
        {
            _context = dbContext;
            _fileService = fileService;
            _permService = permService;
        }

        public async Task<TicketEntity> Execute(UpdateTicketDto dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            Guard.Against.Null(dto.TicketId, nameof(dto.TicketId));

            var ticket = await _context.Tickets
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);

            Guard.Against.Null(ticket, $"Ticket with ID {dto.TicketId} does not exist.");

            var currentUser = await _permService.GetCurrentUser();

            // Ensure only the creator of the ticket can edit subject, text, and files
            if (ticket.CreatedBy.Id != currentUser.Id)
            {
                throw new AccessDenied("You are not authorized to edit this ticket.");
            }

            //if (!string.IsNullOrEmpty(dto.Subject))
            //{
            //    ticket.Subject = dto.Subject;
            //}

            if (!string.IsNullOrEmpty(dto.Text))
            {
                ticket.Text = dto.Text;
            }

            if (dto.Files != null && dto.Files.Any())
            {
                var newFiles = await _fileService.UploadFiles().Execute(dto.Files);
                ticket.Files.AddRange(newFiles);
            }

            // Ensure only moderators can assign users
            if ((currentUser.Role == UserRoles.Admin) && dto.AssignUserId != null)
            {
                var userToAssign = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.AssignUserId);

                Guard.Against.Null(userToAssign, message: "User to assign does not exists");

                ticket.Accept(userToAssign);
            }

            await _context.SaveChangesAsync();

            return ticket;
        }
    }
}
