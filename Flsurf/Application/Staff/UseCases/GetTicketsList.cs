using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetTicketsList : BaseUseCase<GetTicketsDto, ICollection<TicketEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetTicketsList(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<TicketEntity>> Execute(GetTicketsDto dto)
        {
            IQueryable<TicketEntity> query = _context.Tickets;
            var currentUser = await _permService.GetCurrentUser();
            var perm = ZedStaffUser.WithId(currentUser.Id).CanReadTicket(ZedTicket.WithWildcard());

            // Фильтрация по владельцу тикета (UserId)
            if (dto.UserId != null)
            {
                query = query.Where(ticket => ticket.CreatedBy.Id == dto.UserId);
            }

            // Фильтрация по теме тикета (Subject)
            if (string.IsNullOrEmpty(dto.Subject))
            {
                await _permService.EnforceCheckPermission(perm);
                query = query.Where(x => x.Subject == dto.Subject);
            }

            // Фильтрация по назначенному пользователю (IsAssignedToMe)
            if (dto.IsAssignedToMe == true)
            {
                await _permService.EnforceCheckPermission(perm);
                query = query.Where(x => x.AssignedUserId == currentUser.Id);
            }

            // Пагинация
            query = query.Paginate(dto.Start, dto.Ends);

            // Возвращаем результат
            return await query.AsNoTracking().ToListAsync();
        }
    }

}
