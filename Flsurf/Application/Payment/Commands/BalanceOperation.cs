using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.UseCases
{
    public class BalanceOperationCommand : BaseCommand
    {
        [Required]
        public Guid WalletId { get; set; }
        [Required]
        public Money Balance { get; set; } = new(0);
        public BalanceOperationType BalanceOperationType { get; set; } = BalanceOperationType.Deposit; 
    }

    public class BalanceOperation : ICommandHandler<BalanceOperationCommand>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;
        private TransactionInnerService _innerService; 

        public BalanceOperation(IApplicationDbContext dbContext, TransactionInnerService txService, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
            _innerService = txService; 
        }

        public async Task<CommandResult> Handle(BalanceOperationCommand dto)
        {
            var owner = await _permService.GetCurrentUser();
            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(owner.Id)
                    .CanAddBalance(ZedWallet.WithId(dto.WalletId)));
            
            var result = await _innerService.BalanceOperation(dto.Balance, dto.WalletId, dto.BalanceOperationType);
            if (!result.IsSuccess)
                return result; 

            await _context.SaveChangesAsync(); 
        
            return result;
        }
    }
}
