using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Flsurf.Application.Payment.UseCases
{
    public class UpdatePurchase : BaseUseCase<UpdatePurchaseDto, PurchaseEntity>
    {
        public UpdatePurchase() { }

        public async Task<PurchaseEntity> Execute(UpdatePurchaseDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
