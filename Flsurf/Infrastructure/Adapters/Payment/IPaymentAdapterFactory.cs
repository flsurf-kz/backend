namespace Flsurf.Infrastructure.Adapters.Payment
{
    public interface IPaymentAdapterFactory
    {
        IPaymentAdapter GetPaymentAdapter(string providerId); 
    }
}
