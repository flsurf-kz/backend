using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class DeleteComment : BaseUseCase<Guid, TicketCommentEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeleteComment(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _permService = permService;
            _context = dbContext;
        }

        public async Task<TicketCommentEntity> Execute(Guid commentId)
        {
            Guard.Against.Null(commentId, nameof(commentId));

            var comment = await _context.TicketComments
                .Include(x => x.CreatedBy)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == commentId);

            Guard.Against.Null(comment, $"Comment with ID {commentId} does not exist.");

            var owner = await _permService.GetCurrentUser(); 

            if (owner.Id != comment.CreatedBy.Id)
            {
                throw new AccessDenied("you are not owner"); 
            }

            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
