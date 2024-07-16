using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Domain.Staff.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetCommentsList : BaseUseCase<GetCommentsDto, List<TicketCommentEntity>>
    {
        private readonly IApplicationDbContext _context;

        public GetCommentsList(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<TicketCommentEntity>> Execute(GetCommentsDto dto)
        {
            List<TicketCommentEntity> comments = [];

            if (dto.UserId != null)
            {
                comments = await _context.TicketComments
                    .Where(x => x.CreatedBy.Id == dto.UserId)
                    .Include(x => x.Files)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else if (dto.TicketId != null)
            {
                comments = await _context.TicketComments
                    .Where(x => x.TicketId == dto.TicketId)
                    .AsNoTracking()
                    .Include(x => x.Files)
                    .ToListAsync();
            }
            else
            {
                throw new Exception("Ticket id and userId are none");
            }

            return comments;
        }
    }
}
