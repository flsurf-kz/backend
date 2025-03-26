using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class FreelancerFinishContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
    }

    public class FreelancerFinishContractHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)
        : ICommandHandler<FreelancerFinishContractCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(FreelancerFinishContractCommand command)
        {
            var user = await _permService.GetCurrentUser();

            if (user.Type != UserTypes.Freelancer)
                return CommandResult.Forbidden("Только фрилансер может инициировать завершение контракта.");

            var contract = await _dbContext.Contracts
                .FirstOrDefaultAsync(c => c.Id == command.ContractId && c.FreelancerId == user.Id);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден или недоступен.", command.ContractId);

            if (contract.Status != ContractStatus.Active)
                return CommandResult.Conflict("Контракт должен быть в статусе 'Active' для завершения.");

            // Переводим в статус PendingFinishApproval
            contract.Status = ContractStatus.PendingFinishApproval;
            contract.AddDomainEvent(new FreelancerFinishedContract(command.ContractId, user.Id));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }
}
