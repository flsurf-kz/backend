﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Staff.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class CreateComment : BaseUseCase<CreateCommentDto, TicketCommentEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;
        private readonly IFileService _fileService;

        public CreateComment(IApplicationDbContext dbContext, IAccessPolicy accessPolicy, IFileService fileService)
        {
            _context = dbContext;
            _fileService = fileService;
            _accessPolicy = accessPolicy;
        }

        public async Task<TicketCommentEntity> Execute(CreateCommentDto dto)
        {
            var ticket = await _context.Tickets
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == dto.TicketId);

            Guard.Against.Null(ticket, message: "Ticket does not exists");

            var byUser = await _accessPolicy.GetCurrentUser();

            if (!await _accessPolicy.IsAllowed(Domain.User.Enums.PermissionEnum.extend, ticket) 
                && ticket.CreatedBy.Id != byUser.Id)
            {
                throw new AccessDenied("ticket is not created by you");
            }
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
