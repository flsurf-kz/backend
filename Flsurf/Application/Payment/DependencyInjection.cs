using Flsurf.Application.Payment.InnerServices;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Services;
using Flsurf.Infrastructure;

namespace Flsurf.Application.Payment
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentApplicationServices(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddScoped<ITransactionService, TransactionService>();
            services.Configure<FeeSettings>(config.GetSection("FeeSettings"));
            services.AddSingleton<FeePolicyService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<TransactionInnerService>(); 

            return services;
        }
    }
}
