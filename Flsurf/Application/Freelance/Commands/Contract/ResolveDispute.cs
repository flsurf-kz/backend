using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public enum DisputeResolutionStrategy
    {
        RefundClient,
        PayFreelancer,
        SplitPayment
    }

    public class ResolveDisputeCommand : BaseCommand
    {
        public Guid DisputeId { get; set; }
        public DisputeResolutionStrategy Strategy { get; set; }
        public string ModeratorComment { get; set; } = string.Empty;
        public bool BlockFreelancerWallet { get; set; } = false;
        public bool BlockClientWallet { get; set; } = false;
        public bool BlockFreelancerOrders { get; set; } = false;
        public DateTime? BlockUntil { get; set; } // Срок блокировки
    }

    public class ResolveDisputeHandler(
    IApplicationDbContext dbContext,
    IPermissionService permService,
    TransactionInnerService transactionService,
    IFreelanceService freelanceService,
    IMessageService messengerService)
    : ICommandHandler<ResolveDisputeCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly TransactionInnerService _transactionService = transactionService;
        private readonly IFreelanceService _freelanceService = freelanceService;
        private readonly IMessageService _messengerService = messengerService;

        public async Task<CommandResult> Handle(ResolveDisputeCommand command)
        {
            var moderator = await _permService.GetCurrentUser();

            var dispute = await _dbContext.Disputes
                .Include(d => d.Contract)
                    .ThenInclude(c => c.Employer)
                .Include(d => d.Contract)
                    .ThenInclude(c => c.Freelancer)
                .FirstOrDefaultAsync(d => d.Id == command.DisputeId);

            if (dispute == null)
                return CommandResult.NotFound("Спор не найден", command.DisputeId);

            if (dispute.Status != DisputeStatus.InReview)
                return CommandResult.BadRequest("Спор уже закрыт или ещё не принят в работу.");

            var contract = dispute.Contract;

            // Получаем кошельки участников
            var freelancerWallet = await _dbContext.Wallets.FirstAsync(w => w.UserId == contract.FreelancerId);
            var clientWallet = await _dbContext.Wallets.FirstAsync(w => w.UserId == contract.EmployerId);

            var amount = contract.BudgetType switch
            {
                BudgetType.Fixed => new Money(contract.Budget ?? 0, freelancerWallet.Currency),
                BudgetType.Hourly => new Money((contract.CostPerHour ?? 0) * contract.TotalHoursWorked, freelancerWallet.Currency),
                _ => throw new DomainException("Неизвестный тип бюджета")
            };

            switch (command.Strategy)
            {
                case DisputeResolutionStrategy.RefundClient:
                    await _transactionService.UnfreezeAmount(amount, freelancerWallet.Id);
                    await _transactionService.Transfer(amount, clientWallet.Id, freelancerWallet.Id, null, null);
                    break;

                case DisputeResolutionStrategy.PayFreelancer:
                    await _transactionService.UnfreezeAmount(amount, freelancerWallet.Id);
                    // Средства уже у фрилансера — просто разморозить
                    break;

                case DisputeResolutionStrategy.SplitPayment:
                    var halfAmount = new Money(amount.Amount / 2, amount.Currency);
                    await _transactionService.UnfreezeAmount(amount, freelancerWallet.Id);
                    await _transactionService.Transfer(halfAmount, clientWallet.Id, freelancerWallet.Id, null, null);
                    break;
            }

            // Применение ограничений
            if (command.BlockFreelancerWallet && command.BlockUntil.HasValue)
            {
                await _freelanceService.ApplyUserRestrictionAsync(
                    contract.FreelancerId, RestrictionType.WalletBlocked, command.BlockUntil.Value, "Решение AMC по спору");
            }

            if (command.BlockClientWallet && command.BlockUntil.HasValue)
            {
                await _freelanceService.ApplyUserRestrictionAsync(
                    contract.EmployerId, RestrictionType.WalletBlocked, command.BlockUntil.Value, "Решение AMC по спору");
            }

            if (command.BlockFreelancerOrders && command.BlockUntil.HasValue)
            {
                await _freelanceService.ApplyUserRestrictionAsync(
                    contract.FreelancerId, RestrictionType.OrdersBlocked, command.BlockUntil.Value, "Решение AMC по спору");
            }

            // Меняем статус спора
            dispute.Status = DisputeStatus.Resolved;
            dispute.ChangeStatus(DisputeStatus.Resolved, command.ModeratorComment);

            dispute.AddDomainEvent(new DisputeResolvedEvent(dispute.Id, moderator.Id, command.Strategy));

            // Уведомляем участников в чате
            await _messengerService.Send().Execute(new SendMessageDto()
            {
                ChatId = dispute.MessengerChatId!.Value,
                Text = $"AMC вынесло решение по спору: {command.Strategy}. Комментарий: {command.ModeratorComment}"
            });

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(dispute.Id);
        }
    }

}
