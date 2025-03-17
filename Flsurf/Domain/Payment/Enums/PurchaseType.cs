using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PurchaseType
    {
        OrderPromotion,       // Покупка повышения заказа (продвижение заказа)
        PremiumSubscription   // Покупка премиум подписки (расширение возможностей фрилансера)
    }
}
