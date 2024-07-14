using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Exceptions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.UseCases
{
    public class HandleTransaction : BaseUseCase<HandleTransactionDto, TransactionEntity>
    {
        private IApplicationDbContext _context;

        public HandleTransaction(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<TransactionEntity> Execute(HandleTransactionDto dto)
        {
            //var transaction = await _context.Transactions
            //    .IncludeStandard()
            //    .FirstOrDefaultAsync(x => x.Id == dto.TransactionId);

            //Guard.Against.Null(transaction, message: "Transaction does not exists");

            //var ok = Enum.TryParse(transaction.Provider.Name, out PaymentProviders provider);
            //if (!ok)
            //    throw new ValidationException($"Transaction: {transaction.Id} does not have provider name");

            //if (transaction.Status == TransactionStatusEnum.Pending && provider != PaymentProviders.Balance)
            //{
            //    throw new PendingGatewayResult(transaction.Id);
            //}

            //var userWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.User.Id == transaction.CreatedByUser.Id);

            //Guard.Against.Null(userWallet, message: "User wallet");

            //var purchase = await _context.Purchases
            //    .IncludeStandard()
            //    .FirstOrDefaultAsync(x => x.Transaction.Id == transaction.Id);

            //Guard.Against.Null(purchase, message: "Purchase is not bound to transaction");

            //var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == purchase.Product.Id);

            //Guard.Against.Null(product, message: "Product does not exists");

            //var productOwnerWallet = await _context.Wallets
            //    .FirstOrDefaultAsync(x => x.User.Id == product.CreatedById);

            //Guard.Against.Null(productOwnerWallet, message: "Product owner wallet does not exists");

            //if (productOwnerWallet.UserId == userWallet.UserId)
            //{
            //    throw new Exception("Product owner and buyer are same user");
            //}

            //userWallet.ConfirmTransaction(transaction, productOwnerWallet);

            //_context.Wallets.Update(productOwnerWallet);
            //_context.Wallets.Update(userWallet);
            //_context.Transactions.Update(transaction);
            //await _context.SaveChangesAsync();

            //return transaction;
            throw new NotImplementedException();
        }
    }
}
