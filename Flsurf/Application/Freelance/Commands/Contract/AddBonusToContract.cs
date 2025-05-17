// --- Flsurf.Application.Freelance.Commands.Contract.AddBonusToContractCommand.cs ---
using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums; // For BonusType
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore; // For Money

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class AddBonusToContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public decimal AmountValue { get; set; } // e.g., 100.00
        public CurrencyEnum Currency { get; set; } 
        public string Description { get; set; } = null!;
        public BonusType Type { get; set; }
    }

    public class AddBonusToContractHandler : ICommandHandler<AddBonusToContractCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public AddBonusToContractHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(AddBonusToContractCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            if (currentUser == null) // Should ideally not happen if auth is in place
            {
                return CommandResult.Forbidden("User not authenticated.");
            }

            // Typically, only clients/employers can add bonuses
            if (currentUser.Type != UserTypes.Client)
            {
                return CommandResult.Forbidden("Only clients can add bonuses to contracts.");
            }

            var contract = await _dbContext.Contracts
                .Include(c => c.Bonuses) // Include Bonuses to potentially add to the collection in memory
                .FirstOrDefaultAsync(c => c.Id == command.ContractId);

            if (contract == null)
            {
                return CommandResult.NotFound("Contract not found.", command.ContractId);
            }

            // Verify that the current user is the employer for this contract
            if (contract.EmployerId != currentUser.Id)
            {
                return CommandResult.Forbidden("You are not authorized to add a bonus to this contract.");
            }

            // Optional: Add checks for contract status (e.g., bonus only on active/completed contracts)
            if (contract.Status != ContractStatus.Active && contract.Status != ContractStatus.Completed)
            {
                return CommandResult.BadRequest("Bonuses can only be added to active or completed contracts.");
            }

            Money bonusAmount;
            try
            {
                // Assuming Money constructor validates currency etc.
                bonusAmount = new Money(command.AmountValue, command.Currency);
            }
            catch (ArgumentException ex) // Or whatever exception Money might throw for invalid input
            {
                return CommandResult.BadRequest($"Invalid amount or currency: {ex.Message}");
            }

            var bonusEntity = BonusEntity.Create(
                contractId: contract.Id,
                amount: bonusAmount,
                description: command.Description,
                type: command.Type
            );

            _dbContext.Bonuses.Add(bonusEntity);
            // contract.Bonuses.Add(bonusEntity); // EF Core typically handles this via ContractId relationship

            // TODO: Add domain event if needed, e.g., new BonusAddedToContractEvent(bonusEntity)
            // bonusEntity.AddDomainEvent(new BonusAddedToContractEvent(bonusEntity));

            await _dbContext.SaveChangesAsync();


            return CommandResult.Success(bonusEntity.Id);
        }
    }
}