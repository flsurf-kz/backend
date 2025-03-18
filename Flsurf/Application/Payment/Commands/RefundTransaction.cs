using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Commands
{
    public class RefundTransactionCommand : BaseCommand
    {
        [Required]
        public Guid TransactionId { get; set; }

        public bool IsContractCancellation { get; set; } = false;
    }


    public class RefundTransaction : ICommandHandler<RefundTransactionCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly TransactionInnerService _innerService;

        public RefundTransaction(
            IApplicationDbContext dbContext,
            IPermissionService permService,
            TransactionInnerService service)
        {
            _context = dbContext;
            _permService = permService;
            _innerService = service; 
        }

        public async Task<CommandResult> Handle(RefundTransactionCommand dto)
        {
            var currentUser = await _permService.GetCurrentUser();

            // Проверка прав
            await _permService.EnforceCheckPermission(
                ZedPaymentUser.WithId(currentUser.Id)
                    .CanRefundTransaction(ZedTransaction.WithId(dto.TransactionId)));
            try
            {
                var result = await _innerService.Refund(dto.TransactionId);
                if (!result.IsSuccess)
                {
                    return result; 
                }
            } catch (DomainException ex)
            {
                return CommandResult.Conflict(ex.Message); 
            }

            await _context.SaveChangesAsync();

            return CommandResult.Success(dto.TransactionId);
        }
    }

}
