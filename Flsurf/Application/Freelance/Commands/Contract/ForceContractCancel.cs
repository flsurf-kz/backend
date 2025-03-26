using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class ForceContractCancelCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ForceContractCancelHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService,
        TransactionInnerService transactionService)
        : ICommandHandler<ForceContractCancelCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly TransactionInnerService _transactionService = transactionService;

        public async Task<CommandResult> Handle(ForceContractCancelCommand command)
        {
            var user = await _permService.GetCurrentUser();

            // Проверка через AuthZed как ты показывал
            bool hasPermission = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanCancelContract(ZedContract.WithId(command.ContractId)));

            if (!hasPermission)
            {
                return CommandResult.Forbidden("Нет прав для отмены контракта.");
            }

            var contract = await _dbContext.Contracts
                .Include(c => c.WorkSessions)
                .FirstOrDefaultAsync(c => c.Id == command.ContractId);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден.", command.ContractId);

            if (contract.Status == ContractStatus.Cancelled || contract.Status == ContractStatus.Completed)
                return CommandResult.BadRequest("Контракт уже завершён или отменён.");

            var freelancerWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.FreelancerId);
            var clientWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.EmployerId);

            if (freelancerWallet == null || clientWallet == null)
                return CommandResult.BadRequest("Кошельки не найдены.");

            Money refundAmount;

            if (contract.BudgetType == BudgetType.Fixed)
            {
                refundAmount = contract.Budget ?? new Money(0); 
            }
            else if (contract.BudgetType == BudgetType.Hourly)
            {
                var totalWorkedHours = contract.TotalHoursWorked;
                var totalPaidHours = 2; // предположим, что заморожено за 2 часа заранее

                var remainingHours = totalPaidHours - totalWorkedHours;
                if (remainingHours < 0) remainingHours = 0;

                refundAmount = contract.CostPerHour ?? new Money(0) * remainingHours;
            }
            else
            {
                return CommandResult.BadRequest("Неизвестный тип бюджета.");
            }

            if (refundAmount.Amount > 0)
            {
                // Шаг 1: Размораживаем деньги на кошельке фрилансера
                var unfreezeResult = await _transactionService.UnfreezeAmount(refundAmount, freelancerWallet.Id);
                if (!unfreezeResult.IsSuccess)
                    return CommandResult.BadRequest("Не удалось разморозить средства на кошельке фрилансера.");

                // Шаг 2: Переводим деньги обратно клиенту (атомарно через твой сервис)
                var transferResult = await _transactionService.Transfer(
                    refundAmount,
                    recieverWalletId: clientWallet.Id,
                    senderWalletId: freelancerWallet.Id,
                    feePolicy: null,
                    freezeForDays: null
                );

                if (!transferResult.IsSuccess)
                    return CommandResult.BadRequest("Не удалось вернуть средства клиенту.");
            }

            // Финализируем контракт
            contract.Status = ContractStatus.Cancelled;
            contract.EndDate = DateTime.UtcNow;
            contract.PauseReason = command.Reason;
            contract.IsPaused = false;

            contract.AddDomainEvent(new ContractForceCancelledEvent(contract.Id, user.Id, command.Reason));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }


}
