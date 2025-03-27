namespace Flsurf.Infrastructure.Adapters.Payment.Systems
{
    public class TestPaymentAdapter : IPaymentAdapter
    {
        public TestPaymentAdapter() { }

        public Task<PaymentResult> InitPayment(PaymentPayload payload)
        {
            return Task.FromResult(new PaymentResult() { });
        }

        public Task<PaymentStatus> CheckPaymentStatus(string paymentId)
        {
            return Task.FromResult(PaymentStatus.Completed);
        }

        public Task<PaymentResult> RefundPayment(string paymentId, decimal amount)
        {
            return Task.FromResult(new PaymentResult() { });
        }
    }
}
