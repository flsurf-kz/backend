using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Payment.Entities;

// ─────────────────────────────────────────────────────────────
//   Новый Aggregate Root:   PaymentMethodEntity
// ─────────────────────────────────────────────────────────────
public class PaymentMethodEntity : BaseAuditableEntity
{
    // связи
    public Guid UserId { get; private set; }
    public UserEntity User { get; private set; } = null!;
    public Guid ProviderId { get; private set; }          // FK → TransactionProviderEntity
    public TransactionProviderEntity Provider { get; private set; } = null!;

    // неизменяемые свойства после создания
    public string Token { get; private set; } = null!; // Stripe/Kaspi token
    public string Last4 { get; private set; } = null!;
    public string Brand { get; private set; } = null!; // Visa / MC / Kaspi
    public int ExpMonth { get; private set; }
    public int ExpYear { get; private set; }

    // управляемые пользователем
    public bool IsDefault { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    private PaymentMethodEntity() { }                      // EF

    // Фабрика
    public static PaymentMethodEntity Create(
        Guid userId,
        TransactionProviderEntity provider,
        string token,
        string last4,
        string brand,
        int expMonth,
        int expYear,
        bool isDefault)
    {
        return new PaymentMethodEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProviderId = provider.Id,
            Provider = provider,
            Token = token,
            Last4 = last4,
            Brand = brand,
            ExpMonth = expMonth,
            ExpYear = expYear,
            IsDefault = isDefault
        };
    }

    public void Deactivate() => IsActive = false;
    public void MakeDefault() => IsDefault = true;
}
