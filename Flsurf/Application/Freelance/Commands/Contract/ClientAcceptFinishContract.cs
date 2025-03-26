using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects; 
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class ClientAcceptFinishContractCommand : BaseCommand
    { 
        public Guid ContractId { get; }
    }


    public class ClientAcceptFinishContractHandler(
        IApplicationDbContext dbContext, 
        IPermissionService permService, 
        TransactionInnerService innerService) : ICommandHandler<ClientAcceptFinishContractCommand>
    {
        private readonly TransactionInnerService _txService = innerService; 
        private readonly IPermissionService _permissionService = permService;
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<CommandResult> Handle(ClientAcceptFinishContractCommand command)
        {
            var user = await _permissionService.GetCurrentUser();

            var contract = await _dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == command.ContractId);

            if (contract == null)
                return CommandResult.NotFound("", command.ContractId);

            var freelancerWallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == contract.FreelancerId); 
            var clientWallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == contract.EmployerId);

            if (freelancerWallet == null || clientWallet == null)
                return CommandResult.NotFound("", contract.FreelancerId);

            if (contract.BudgetType == BudgetType.Fixed)
            {
                await _txService.UnfreezeAmount(contract.Budget ?? new Money(0), freelancerWallet.Id);
                await _txService.FreezeAmount(contract.Budget ?? new Money(0), freelancerWallet.Id, 14);
            } else if (contract.BudgetType == BudgetType.Hourly)
            {
                // хуйня все вырубай
            }

            contract.Finish();

            await _dbContext.SaveChangesAsync(); 

            return CommandResult.Success(); 
        }
    }
}

