using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Commands
{
    public class TransferFundsCommand : BaseCommand
    {
        public Guid SenderWalletId { get; set; }
        public Guid ReceiverWalletId { get; set; }
        public decimal Amount { get; set; }
        public int? FreezeForDays { get; set; }  // Если нужно заморозить средства на определенное время
    }

    public class TransferFundsHandler : ICommandHandler<TransferFundsCommand>
    {
        private readonly IApplicationDbContext _context;

        public TransferFundsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResult> Handle(TransferFundsCommand command)
        {
            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == command.SenderWalletId, cancellationToken);
            var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == command.ReceiverWalletId, cancellationToken);

            if (senderWallet == null || receiverWallet == null)
                return CommandResult.NotFound("Кошелек отправителя или получателя не найден", command.SenderWalletId);

            try
            {
                // Передаем feePolicy как null, если не требуется комиссия.
                senderWallet.Transfer(command.Amount, receiverWallet, feePolicy: null, freezeForDays: command.FreezeForDays);
            }
            catch (Exception ex)
            {
                return CommandResult.BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(command.ReceiverWalletId);
        }
    }
}
