using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Flsurf.Application.Payment.UseCases
{
    public class CreateTransaction : BaseUseCase<CreateTransactionDto, TransactionEntity>
    {
        public CreateTransaction() { }

        public async Task<TransactionEntity> Execute(CreateTransactionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
