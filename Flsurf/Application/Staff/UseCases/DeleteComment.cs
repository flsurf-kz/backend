using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Staff.UseCases
{
    public class DeleteComment(IApplicationDbContext dbContext, IPermissionService permService) : BaseUseCase<Guid, TicketCommentEntity>
    {
        private readonly IApplicationDbContext _context = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<TicketCommentEntity> Execute(Guid commentId)
        {
            Guard.Against.Null(commentId, nameof(commentId));

            var comment = await _context.TicketComments.FindAsync(commentId);

            Guard.Against.Null(comment, $"Comment with ID {commentId} does not exist.");
 

            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
