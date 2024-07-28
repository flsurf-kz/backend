using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class CreateTicket : BaseUseCase<CreateTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly IFileService _fileService;

        public CreateTicket(IApplicationDbContext dbContext, IPermissionService permService, IFileService fileService)
        {
            _context = dbContext;
            _fileService = fileService;
            _permService = permService;
        }

        public async Task<TicketEntity> Execute(CreateTicketDto dto)
        {
            var byUser = await _permService.GetCurrentUser();

            var newFiles = await _fileService.UploadFiles().Execute(dto.Files);
            var subject = await _context.TicketSubjects.FirstOrDefaultAsync(x => x.Id == dto.SubjectId);

            Guard.Against.Null(subject, message: "Subject does not exists"); 

            var ticket = TicketEntity.Create(dto.Text, subject, byUser, (List<FileEntity>)newFiles);

            await _context.Tickets.AddAsync(ticket);

            var staffUser = ZedStaffUser.WithId(byUser.Id);
            var zedTicket = ZedTicket.WithId(ticket.Id); 

            await _permService.AddRelationships(
                staffUser.CanReadTicket(zedTicket),
                staffUser.CanUpdateTicket(zedTicket),
                staffUser.CanCloseTicket(zedTicket)); 
            await _context.SaveChangesAsync();

            return ticket;
        }
    }
}
