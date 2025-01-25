using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
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
            // Формируем базовый запрос
            var query = _context.Tickets.AsQueryable();

            var currentUser = await _permService.GetCurrentUser();

            // Проверка: пользователь может читать тикеты
            if (!await _permService.CheckPermission(
                ZedStaffUser.WithId(currentUser.Id).CanReadTicket(ZedTicket.WithWildcard())
            ))
            {
                throw new AccessDenied("You do not have permission to view tickets.");
            }

            // Фильтрация по владельцу тикета (UserId)
            if (dto.UserId != null)
            {
                query = query.Where(ticket => ticket.CreatedBy.Id == dto.UserId);
            }

            // Фильтрация по теме тикета (SubjectId)
            if (dto.SubjectId != null)
            {
                query = query.Where(ticket => ticket.Subject.Id == dto.SubjectId);
            }

            // Фильтрация по назначенному пользователю (IsAssignedToMe)
            if (dto.IsAssignedToMe == true)
            {
                query = query.Where(ticket => ticket.AssignedUser.Id == currentUser.Id);
            }

            // Пагинация
            query = query.Paginate(dto.Start, dto.Ends);

            // Возвращаем результат
            return await query.AsNoTracking().ToListAsync();
        }
    }

}
