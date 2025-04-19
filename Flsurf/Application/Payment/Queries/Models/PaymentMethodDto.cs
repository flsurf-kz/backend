namespace Flsurf.Application.Payment.Queries.Models
{
    public record PaymentMethodDto(
        Guid Id,
        Guid ProviderId,
        string Brand,
        string MaskedPan,
        int ExpMonth,
        int ExpYear,
        bool IsDefault);
}
