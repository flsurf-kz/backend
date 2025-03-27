using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Messanging.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class AcceptDisputeCommand : BaseCommand
    {
        public Guid DisputeId { get; set; }
    }


    public class AcceptDisputeHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService,
        IStaffService staffService,
        IChatService messengerService,
        TransactionInnerService transactionService)
        : ICommandHandler<AcceptDisputeCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly IStaffService _staffService = staffService;
        private readonly IChatService _messengerService = messengerService;
        private readonly TransactionInnerService _transactionService = transactionService;

        public async Task<CommandResult> Handle(AcceptDisputeCommand command)
        {
            var moderator = await _permService.GetCurrentUser();

            var dispute = await _dbContext.Disputes
                .Include(d => d.Contract)
                    .ThenInclude(c => c.Employer)
                .Include(d => d.Contract)
                    .ThenInclude(c => c.Freelancer)
                .FirstOrDefaultAsync(d => d.Id == command.DisputeId);

            if (dispute == null)
                return CommandResult.NotFound("Спор не найден.", command.DisputeId);

            if (dispute.Status != DisputeStatus.Pending)
                return CommandResult.BadRequest("Этот спор уже взят в работу или решён.");

            // 1. Назначаем тикет на модератора
            await _staffService.UpdateTicket().Execute(new UpdateTicketDto() {
                TicketId = dispute.StaffTicketId!.Value,
                AssignUserId = moderator.Id 
            });

            // 2. Создание общего чата (модератор + клиент + фрилансер)
            var generalChatParticipants = new List<Guid>
            {
                moderator.Id,
                dispute.Contract.EmployerId,
                dispute.Contract.FreelancerId
            };

            var generalChatId = await _messengerService.Create().Execute(new Messaging.Dto.CreateChatDto() {
                Name = $"Спор по контракту №{dispute.Contract.Id}",
                UserIds = generalChatParticipants,
                type = ChatTypes.Group,
            });

            dispute.MessengerChatId = generalChatId;

            // 3. Создание приватного чата с инициатором и модератором AMC
            var privateChatParticipants = new List<Guid> { moderator.Id, dispute.InitiatorId };
            var privateChatId = await _messengerService.Create().Execute( new Messaging.Dto.CreateChatDto() {
                Name = $"ЛС по спору №{dispute.Contract.Id}",
                UserIds = privateChatParticipants, 
                type = ChatTypes.Support, 
            });

            // 4. Заморозка финансовых средств сторон контракта
            await FreezeContractFunds(dispute.Contract);

            // 5. Изменение статуса спора на InReview
            dispute.Status = DisputeStatus.InReview;
            dispute.ChangeStatus(DisputeStatus.InReview, $"Взят в работу модератором {moderator.Name}");

            dispute.AddDomainEvent(new DisputeAcceptedEvent(dispute.Id, moderator.Id));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(dispute.Id);
        }

        private async Task FreezeContractFunds(ContractEntity contract)
        {
            var freelancerWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.FreelancerId);
            var clientWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == contract.EmployerId);

            Guard.Against.NotFound(contract.FreelancerId, freelancerWallet); // something have gone very wrong 
            Guard.Against.NotFound(contract.FreelancerId, clientWallet);

            Money freezeAmount;

            if (contract.BudgetType == BudgetType.Fixed && contract.Budget != Money.Null())
            {
                freezeAmount = contract.Budget; 
            }
            else if (contract.BudgetType == BudgetType.Hourly && contract.CostPerHour != Money.Null())
            {
                freezeAmount = contract.CostPerHour * 2; 
            }
            else
            {
                throw new DomainException("Неизвестный тип бюджета или отсутствует сумма.");
            }

            // Замораживаем средства у обеих сторон (если ещё не заморожены)
            await _transactionService.FreezeAmount(freezeAmount, freelancerWallet.Id, frozenTimeInDays: 999);
            await _transactionService.FreezeAmount(freezeAmount, clientWallet.Id, frozenTimeInDays: 999);
        }
    }
}
