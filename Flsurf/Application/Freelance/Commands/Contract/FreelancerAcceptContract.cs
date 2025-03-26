using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class FreelancerAcceptContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
    }


    public class FreelancerAcceptContractHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService,
        TransactionInnerService transactionService)
        : ICommandHandler<FreelancerAcceptContractCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly TransactionInnerService _transactionService = transactionService;

        public async Task<CommandResult> Handle(FreelancerAcceptContractCommand command)
        {
            var freelancer = await _permService.GetCurrentUser();

            if (freelancer.Type != UserTypes.Freelancer)
                return CommandResult.Forbidden("Только фрилансер может принять контракт.");

            var contract = await _dbContext.Contracts
                .Include(c => c.Employer)
                .Include(c => c.Freelancer)
                .FirstOrDefaultAsync(c => c.Id == command.ContractId && c.FreelancerId == freelancer.Id);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден или недоступен.", command.ContractId);

            if (contract.Status != ContractStatus.PendingApproval)
                return CommandResult.Conflict("Контракт уже принят или неактуален.");

            var clientWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.EmployerId);
            var freelancerWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == freelancer.Id);

            if (clientWallet == null || freelancerWallet == null)
                return CommandResult.NotFound("Кошельки не найдены.", contract.EmployerId);

            Money transferAmount;
            int? freezeDays;

            if (contract.BudgetType == BudgetType.Fixed)
            {
                if (contract.Budget is null)
                    return CommandResult.BadRequest("Не указана сумма фиксированного бюджета.");

                transferAmount = new Money(contract.Budget);
                freezeDays = null; // бессрочно до завершения контракта
            }
            else if (contract.BudgetType == BudgetType.Hourly)
            {
                if (contract.CostPerHour is null)
                    return CommandResult.BadRequest("Не указана почасовая ставка.");

                transferAmount = new Money(contract.CostPerHour * 2);
                freezeDays = 14; // замораживаем на 14 дней
            }
            else
            {
                return CommandResult.BadRequest("Неизвестный тип бюджета.");
            }

            // Проводим внутренний перевод с заморозкой денег у фрилансера
            var transferResult = await _transactionService.Transfer(
                transferAmount,
                recieverWalletId: freelancerWallet.Id,
                senderWalletId: clientWallet.Id,
                feePolicy: null, // без комиссии для внутреннего перевода
                freezeForDays: freezeDays);

            if (!transferResult.IsSuccess)
                return transferResult; // если ошибка при переводе, возвращаем ошибку

            // Обновляем статус контракта и Job
            contract.Status = ContractStatus.Active;
            contract.StartDate = DateTime.UtcNow;

            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.ContractId == contract.Id);
            if (job != null)
            {
                job.Status = JobStatus.InContract;
            }

            // Событие подписания контракта
            contract.AddDomainEvent(new ContractSignedEvent(contract.Id, freelancer.Id));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }

}
