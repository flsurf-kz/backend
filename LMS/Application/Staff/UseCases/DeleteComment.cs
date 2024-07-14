using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;

namespace Flsurf.Application.Staff.UseCases
{
    public class DeleteComment : BaseUseCase<Guid, TicketCommentEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;

        public DeleteComment(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _accessPolicy = accessPolicy;
            _context = dbContext;
        }

        public async Task<TicketCommentEntity> Execute(Guid commentId)
        {
            Guard.Against.Null(commentId, nameof(commentId));

            var comment = await _context.TicketComments.FindAsync(commentId);

            Guard.Against.Null(comment, $"Comment with ID {commentId} does not exist.");

            await _accessPolicy.EnforceIsAllowed(PermissionEnum.delete, comment);

            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
