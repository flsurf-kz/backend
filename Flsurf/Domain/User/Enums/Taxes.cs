using System.Text.Json.Serialization;

namespace Flsurf.Domain.User.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LegalStatus
    {
        Individual,
        SoleProprietor,
        Entity
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaxRegime
    {
        OSNO,           // основная (RU)
        USN_Income6,    // УСН 6% доходы (RU)
        USN_Profit15,   // УСН 15% разница (RU)
        Patent,         // патент (RU)
        Simplified,     // упрощёнка (КЗ, BY, UA, UZ, KG)
        General         // общая (КЗ, BY, UA, UZ, KG)
    }
}
