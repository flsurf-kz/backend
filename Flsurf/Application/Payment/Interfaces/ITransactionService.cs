using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Interfaces
{
    public interface ITransactionService
    {
        HandleTransaction HandleTransaction();
        GetTransactionsList GetTransactionsList();
        UpdateTransaction UpdateTransaction();
        GetTransactionProviders GetTransactionProviders();
        HandleGatewayResult HandleGatewayResult();
    }
}
