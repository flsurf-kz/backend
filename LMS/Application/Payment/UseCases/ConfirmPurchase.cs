using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Application.Payment.UseCases
{
    public class ConfirmPurchase : BaseUseCase<ConfirmPurchaseDto, PurchaseEntity>
    {
        private IApplicationDbContext _context;
        private IUser _user;
        private ILogger _logger;

        public ConfirmPurchase(IApplicationDbContext dbContext, IUser user, ILogger<ConfirmPurchase> logger)
        {
            _context = dbContext;
            _user = user;
            _logger = logger;
        }

        public async Task<PurchaseEntity> Execute(ConfirmPurchaseDto dto)
        {
            return new(); 
        }
    }
}
