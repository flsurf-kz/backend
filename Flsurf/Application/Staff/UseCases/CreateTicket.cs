using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class CreateTicket : BaseUseCase<CreateTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;
        private readonly IFileService _fileService;

        public CreateTicket(IApplicationDbContext dbContext, IAccessPolicy accessPolicy, IFileService fileService)
        {
            _context = dbContext;
            _fileService = fileService;
            _accessPolicy = accessPolicy;
        }

        public async Task<TicketEntity> Execute(CreateTicketDto dto)
        {
            var byUser = await _accessPolicy.GetCurrentUser();

            var newFiles = await _fileService.UploadFiles().Execute(dto.Files);
            var subject = await _context.TicketSubjects.FirstOrDefaultAsync(x => x.Id == dto.SubjectId);

            Guard.Against.Null(subject, message: "Subject does not exists"); 

            var ticket = TicketEntity.Create(dto.Text, subject, byUser, (List<FileEntity>)newFiles);

            await _context.Tickets.AddAsync(ticket);

            byUser.Permissions.AddPermissionWithCode(
                ticket, PermissionEnum.write, PermissionEnum.edit, PermissionEnum.read); 
            await _context.SaveChangesAsync();

            return ticket;
        }
    }
}
