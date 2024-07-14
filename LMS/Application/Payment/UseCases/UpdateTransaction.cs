using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Flsurf.Application.Payment.UseCases
{
    public class UpdateTransaction : BaseUseCase<UpdateTransactionDto, TransactionEntity>
    {
        public UpdateTransaction() { }

        public async Task<TransactionEntity> Execute(UpdateTransactionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
