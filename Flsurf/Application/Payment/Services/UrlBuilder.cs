using Flsurf.Application.Payment.Interfaces;

namespace Flsurf.Application.Payment.Services
{
    public class UrlBuilder : IUrlBuilder
    {
        private readonly string _front = "https://flsurf.ru";
        public string Success(Guid txId) => $"{_front}/api/payment/success?tx={txId}";
    }
}
