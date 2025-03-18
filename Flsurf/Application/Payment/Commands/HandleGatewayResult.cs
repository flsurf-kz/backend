using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Flsurf.Application.Payment.UseCases
{
    public class GatewayResultDto : BaseCommand
    {
        [Required]
        public bool Success { get; set; } = false;
        [Required]
        public string CustomId { get; set; } = string.Empty;
        [Required]
        public decimal Fee { get; set; }
        [Required]
        public CurrencyEnum CurrencyIn { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionId { get; set; } = string.Empty;
        [Required]
        public string Custom { get; set; } = string.Empty;
    }

    public class HandleGatewayResult : ICommandHandler<GatewayResultDto>
    {
        private IApplicationDbContext _context;
        // transaction id: {tx_uuid}:{purchase_uuid}
        private readonly Regex transactionIdRegex = new Regex(
            "[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}");

        public HandleGatewayResult(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        private (Guid, string) ExtractTxId(string customId, string custom)
        {
            Match match = transactionIdRegex.Match(customId);
            if (!match.Success)
                match = transactionIdRegex.Match(custom);

            if (!match.Success)
                return (
                    Guid.Empty,
                    "Custom or Custom id does not have any id" + custom.ToString() + customId); 

            return (Guid.Parse(match.Value), "");
        }

        public async Task<CommandResult> Handle(GatewayResultDto dto)
        {
            if (string.IsNullOrEmpty(dto.CustomId) && !transactionIdRegex.IsMatch(dto.Custom))
                return CommandResult.UnprocessableEntity(
                    "Транзакция не правильная: " + dto.Custom.ToString() + dto.CustomId); 

            var (txId, err) = ExtractTxId(dto.CustomId, dto.Custom);

            if (txId == Guid.Empty)
                return CommandResult.UnprocessableEntity(err);

            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == txId);

            if (transaction == null)
                return CommandResult.NotFound("не найдена транзакция", txId); 

            transaction.ConfirmFromGateway();

            await _context.SaveChangesAsync();

            return CommandResult.Success(transaction.Id);
        }
    }
}
