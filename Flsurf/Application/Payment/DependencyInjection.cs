using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Services;

namespace Flsurf.Application.Payment
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<PaymentAdapterFactory>();

            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IWalletService, WalletService>();

            return services;
        }
    }
}
