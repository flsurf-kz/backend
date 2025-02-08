using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class CreateComment(
        IApplicationDbContext dbContext, IPermissionService permService, IFileService fileService
    ) : BaseUseCase<CreateCommentDto, TicketCommentEntity>
    {
        private readonly IApplicationDbContext _context = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly IFileService _fileService = fileService;

        public async Task<TicketCommentEntity> Execute(CreateCommentDto dto)
        {
            var ticket = await _context.Tickets
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);

            Guard.Against.Null(ticket, message: "Ticket does not exists");

            var byUser = await _permService.GetCurrentUser();
            await _permService.EnforceCheckPermission(
                ZedStaffUser
                    .WithId(byUser.Id)
                    .CanAddComment(ZedTicket.WithId(ticket.Id))); 

            var newFiles = await _fileService.UploadFiles().Execute(dto.Files);

            TicketCommentEntity comment;
            if (dto.ParentCommentId != null)
            {
                comment = ticket.AddComment(byUser, dto.Text, newFiles, (Guid)dto.ParentCommentId);
            }
            else
            {
                comment = ticket.AddComment(byUser, dto.Text, newFiles);
            }

            await _context.TicketComments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
