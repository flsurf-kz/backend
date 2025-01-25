using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class UpdateTicket : BaseUseCase<UpdateTicketDto, TicketEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly IFileService _fileService;

        public UpdateTicket(IApplicationDbContext dbContext, IPermissionService permService, IFileService fileService)
        {
            _context = dbContext;
            _fileService = fileService;
            _permService = permService;
        }

        public async Task<TicketEntity> Execute(UpdateTicketDto dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            Guard.Against.Null(dto.TicketId, nameof(dto.TicketId));

            // Получаем тикет из базы данных
            var ticket = await _context.Tickets
                .Include(t => t.Files)
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId);

            Guard.Against.Null(ticket, $"Ticket with ID {dto.TicketId} does not exist.");

            var currentUser = await _permService.GetCurrentUser();

            // Проверяем, может ли пользователь обновлять тикет
            if (!await _permService.CheckPermission(
                ZedStaffUser.WithId(currentUser.Id).CanUpdateTicket(ZedTicket.WithId(ticket.Id))
            ))
            {
                throw new AccessDenied("You do not have permission to update this ticket.");
            }

            // Обновление текстового контента тикета
            if (!string.IsNullOrEmpty(dto.Text))
            {
                ticket.Text = dto.Text;
            }

            // Обработка файлов
            if (dto.Files != null && dto.Files.Any())
            {
                var newFiles = await _fileService.UploadFiles().Execute(dto.Files);
                ticket.Files.AddRange(newFiles);
            }

            // Назначение пользователя (только для администраторов)
            if (dto.AssignUserId.HasValue)
            {
                var userToAssign = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.AssignUserId.Value);
                Guard.Against.Null(userToAssign, message: "User to assign does not exist.");

                ticket.Accept(userToAssign);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return ticket;
        }
    }

}
