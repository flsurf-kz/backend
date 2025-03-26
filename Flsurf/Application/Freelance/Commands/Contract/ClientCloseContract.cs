using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class ClientCloseContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public string Reason { get; set; } = "Закрытие контракта клиентом";
    }

    public class ClientCloseContractHandler(
    IApplicationDbContext dbContext,
    IPermissionService permService,
    TransactionInnerService transactionService)
    : ICommandHandler<ClientCloseContractCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly TransactionInnerService _transactionService = transactionService;

        private const decimal ClientPenaltyRate = 0.10m; // 10%

        public async Task<CommandResult> Handle(ClientCloseContractCommand command)
        {
            var user = await _permService.GetCurrentUser();

            var contract = await _dbContext.Contracts
                .Include(c => c.WorkSessions)
                .FirstOrDefaultAsync(c => c.Id == command.ContractId && c.EmployerId == user.Id);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден или вам недоступен.", command.ContractId);

            if (contract.Status is ContractStatus.Completed or ContractStatus.Cancelled)
                return CommandResult.BadRequest("Контракт уже завершён или отменён.");

            var freelancerWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.FreelancerId);
            var clientWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.EmployerId);

            if (freelancerWallet == null || clientWallet == null)
                return CommandResult.NotFound("Не найдены кошельки участников.", contract.EmployerId);

            Money refundableAmount;

            if (contract.BudgetType == BudgetType.Fixed)
            {
                refundableAmount = contract.Budget ?? new Money(0);
            }
            else if (contract.BudgetType == BudgetType.Hourly)
            {
                if (contract.CostPerHour is null)
                    return CommandResult.BadRequest("Не указана почасовая ставка.");

                var totalWorkedHours = contract.TotalHoursWorked;
                var prepaidHours = 2m;

                var remainingHours = prepaidHours - totalWorkedHours;
                if (remainingHours <= 0)
                {
                    refundableAmount = new Money(0, contract.CostPerHour.Currency);
                }
                else
                {
                    refundableAmount = contract.CostPerHour * remainingHours;
                }
            }
            else
            {
                return CommandResult.BadRequest("Неизвестный тип бюджета.");
            }

            if (!refundableAmount.IsZero())
            {
                // Штраф 10% остаётся у фрилансера
                var penaltyAmount = refundableAmount * ClientPenaltyRate;
                var amountAfterPenalty = refundableAmount - penaltyAmount;

                // 1. Размораживаем всю сумму на кошельке фрилансера
                var unfreezeResult = await _transactionService.UnfreezeAmount(refundableAmount, freelancerWallet.Id);
                if (!unfreezeResult.IsSuccess)
                    return CommandResult.BadRequest("Не удалось разморозить средства фрилансера.");

                // 2. Переводим сумму после удержания обратно клиенту
                var refundResult = await _transactionService.Transfer(
                    amountAfterPenalty,
                    recieverWalletId: clientWallet.Id,
                    senderWalletId: freelancerWallet.Id,
                    feePolicy: null,
                    freezeForDays: null
                );

                if (!refundResult.IsSuccess)
                    return CommandResult.BadRequest("Не удалось перевести возврат клиенту.");
            }

            contract.CancelContract();
            contract.AddDomainEvent(new ContractCancelledByClientEvent(contract.Id, user.Id, command.Reason));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }
}
