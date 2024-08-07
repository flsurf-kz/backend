﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class CreatePurchase : BaseUseCase<CreatePurchaseDto, PurchaseEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUser _user;
        private readonly PaymentAdapterFactory _paymentAdapterFactory;
        private readonly ILogger _logger;

        public CreatePurchase(
            IApplicationDbContext dbContext,
            IUser user,
            PaymentAdapterFactory adapterFactory,
            ILogger<CreatePurchase> logger)
        {
            _logger = logger;
            _paymentAdapterFactory = adapterFactory;
            _user = user;
            _context = dbContext;
        }

        public async Task<PurchaseEntity> Execute(CreatePurchaseDto dto)
        {
            //var product = await _context.Products
            //    .IncludeStandard()
            //    .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            //Guard.Against.Null(product, message: "Product does not exists");
            //if (product.Status != Domain.Market.Enums.ProductStatus.Approved)
            //{
            //    throw new Exception("Product status differs from approved");
            //}

            //var provider = await _context.TransactionProviders.FirstOrDefaultAsync(x => x.Name == dto.ProviderName);

            //Guard.Against.Null(provider, message: "Provider does not exists");

            //var userWallet = await _context.Wallets
            //    .IncludeStandard()
            //    .FirstOrDefaultAsync(x => x.UserId == _user.Id);

            //var productOwnerWallet = await _context.Wallets
            //    .IncludeStandard()
            //    .FirstOrDefaultAsync(x => x.UserId == product.CreatedBy.Id);
            //Guard.Against.Null(userWallet, message: "User wallet does not exists!!");
            //Guard.Against.Null(productOwnerWallet, message: "User wallet does not exists!!");
            //if (userWallet.UserId == productOwnerWallet.UserId)
            //{
            //    throw new Exception("Buyer and Product owner is not allowed to be same user");
            //}

            //var transaction = TransactionEntity.Create(
            //    value: product.CurrentPrice,
            //    fee: new Money(1),
            //    operation: TransactionOperations.Buy,
            //    direction: TransactionDirection.Out,
            //    provider: provider,
            //    user: userWallet.User);

            //var adapter = _paymentAdapterFactory.GetPaymentAdapter(providerId: provider.Name);

            //if (adapter == null)
            //{
            //    throw new NotImplementedException($"Implement {adapter} please");
            //}

            //var result = await adapter.InitPayment(new Infrastructure.Adapters.Payment.PaymentPayload()
            //{
            //    Currency = nameof(transaction.Amount.Currency),
            //    Amount = transaction.Amount.Amount + CalculateFee(product.CurrentPrice, provider.Fee == 0 ? 1 : provider.Fee),
            //    AdditionalInfo = { },
            //    Custom = $"Оплата за {product.Name}",
            //    Name = $"Оплата за {product.Name}",
            //    OrderId = $"{product.Id}:{transaction.Id}",
            //});

            //_logger.LogInformation($"Result of the gatewy: {result}");
            //transaction.Props = TransactionPropsEntity.CreateGatewayProps(
            //        result.LinkUrl,
            //        $"/transaction/{transaction.Id}",
            //        adapter.GetType().Name);

            //_logger.LogInformation($"Result of props: {transaction.Props}");
            //var purchase = PurchaseEntity.Create(userWallet, product, transaction);

            //productOwnerWallet.FreezeAmount(transaction.Amount);
            //userWallet.DecreaseBalance(transaction.Amount);

            //_context.Purchases.Add(purchase);
            //_context.Transactions.Add(transaction);
            //_context.Wallets.UpdateRange([productOwnerWallet, userWallet]);
            //await _context.SaveChangesAsync();

            //return purchase;
            throw new NotImplementedException(); 
        }

        private decimal CalculateFee(Money price, decimal fee)
        {
            return price.Amount / (100 / fee);
        }
    }
}
