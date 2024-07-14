using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IServiceProvider _serviceProvider;

        public PurchaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreatePurchase CreatePurchase()
        {
            return _serviceProvider.GetRequiredService<CreatePurchase>();
        }
        public ConfirmPurchase ConfirmPurchase()
        {
            return _serviceProvider.GetRequiredService<ConfirmPurchase>();
        }
        public GetPurchasesList GetPurchasesList()
        {
            return _serviceProvider.GetRequiredService<GetPurchasesList>();
        }
        public UpdatePurchase UpdatePurchase()
        {
            return _serviceProvider.GetRequiredService<UpdatePurchase>();
        }
        public SolveProblem SolveProblem()
        {
            return _serviceProvider.GetRequiredService<SolveProblem>();
        }
    }
}
