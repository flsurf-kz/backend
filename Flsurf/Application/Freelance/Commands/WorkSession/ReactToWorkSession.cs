using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.WorkSession
{
    public class ReactToWorkSessionCommand : BaseCommand
    {
        public Guid WorkSessionId { get; set; }
        public bool IsApproved { get; set; }
        public string? ClientComment { get; set; }
    }

    public class ReactToWorkSessionHandler : ICommandHandler<ReactToWorkSessionCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly TransactionInnerService _transactionService;

        public ReactToWorkSessionHandler(
            IApplicationDbContext context,
            IPermissionService permService,
            TransactionInnerService transactionService)
        {
            _context = context;
            _permService = permService;
            _transactionService = transactionService;
        }

        public async Task<CommandResult> Handle(ReactToWorkSessionCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();
            if (currentUser.Type != UserTypes.Client)
                return CommandResult.Forbidden("Только клиент может реагировать на сессию.");

            var session = await _context.WorkSessions
                .Include(s => s.Contract)
                .ThenInclude(c => c.Employer) // Для проверки работодателя
                .FirstOrDefaultAsync(s => s.Id == command.WorkSessionId);

            if (session == null)
                return CommandResult.NotFound("Сессия не найдена.", command.WorkSessionId);

            if (session.Contract.EmployerId != currentUser.Id)
                return CommandResult.Forbidden("Вы не являетесь владельцем данного контракта.");

            if (session.Status != WorkSessionStatus.Pending)
                return CommandResult.BadRequest("Нельзя изменить статус уже обработанной сессии.");

            if (command.IsApproved)
            {
                session.ApproveSession();

                // Переводим средства
                // Проверяем кошелёк клиента и фрилансера
                var clientWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == session.Contract.EmployerId);
                if (clientWallet == null)
                    return CommandResult.NotFound("Кошелёк клиента не найден.", session.Contract.EmployerId);

                var freelancerWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == session.FreelancerId);
                if (freelancerWallet == null)
                    return CommandResult.NotFound("Кошелёк фрилансера не найден.", session.FreelancerId);

                var totalEarned = session.TotalEarned();
                if (totalEarned.Amount < 0)
                    return CommandResult.BadRequest("Сумма к переводу некорректна (контракт не почасовой).");

                var transferResult = await _transactionService.Transfer(
                    totalEarned,
                    freelancerWallet.Id,
                    clientWallet.Id,
                    feePolicy: null,
                    freezeForDays: null // При необходимости можно заморозить на N дней
                );
                if (!transferResult.IsSuccess)
                    return CommandResult.BadRequest("Ошибка при переводе средств фрилансеру: " + transferResult.Message);
            }
            else
            {
                session.RejectSession(command.ClientComment ?? string.Empty);
            }

            await _context.SaveChangesAsync();
            return CommandResult.Success(session.Id);
        }
    }
}
