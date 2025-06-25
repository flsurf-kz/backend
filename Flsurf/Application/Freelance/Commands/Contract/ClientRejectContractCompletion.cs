using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class ClientRejectContractCompletionCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ClientRejectContractCompletionHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)
        : ICommandHandler<ClientRejectContractCompletionCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(ClientRejectContractCompletionCommand command)
        {
            var employer = await _permService.GetCurrentUser();

            if (employer.Type != UserTypes.Client)
                return CommandResult.Forbidden("Только заказчик может отправить контракт на доработку.");

            var contract = await _dbContext.Contracts
                .FirstOrDefaultAsync(c => c.Id == command.ContractId && c.EmployerId == employer.Id);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден или недоступен.", command.ContractId);

            if (contract.Status != ContractStatus.PendingFinishApproval)
                return CommandResult.Conflict($"Контракт не ожидает подтверждения завершения. текущий статус: {contract.Status}");

            // Возвращаем в активный статус и записываем причину
            contract.Status = ContractStatus.Active;
            contract.PauseReason = command.Reason;

            contract.AddDomainEvent(new ContractFinishRejected(contract.Id, employer.Id, command.Reason));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }
}
