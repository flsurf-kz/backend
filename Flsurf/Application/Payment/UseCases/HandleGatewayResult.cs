﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Flsurf.Application.Payment.UseCases
{
    public class HandleGatewayResult : BaseUseCase<GatewayResultDto, bool>
    {
        private IApplicationDbContext _context;
        // transaction id: {tx_uuid}:{purchase_uuid}
        private readonly Regex transactionIdRegex = new Regex(
            "[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}");

        public HandleGatewayResult(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        private Guid ExtractTxId(string customId, string custom)
        {
            Match match = transactionIdRegex.Match(customId);
            if (!match.Success)
                match = transactionIdRegex.Match(custom);

            if (!match.Success)
                throw new TransactionInvalid(custom, customId, "Custom or Custom id does not have any id");

            return Guid.Parse(match.Value);
        }

        public async Task<bool> Execute(GatewayResultDto dto)
        {
            if (string.IsNullOrEmpty(dto.CustomId) && !transactionIdRegex.IsMatch(dto.Custom))
                throw new TransactionInvalid(dto.Custom, dto.CustomId);

            Guid txId = ExtractTxId(dto.CustomId, dto.Custom);

            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == txId);

            Guard.Against.Null(transaction, message: "Transaction does not exists");

            transaction.Confirm();

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
