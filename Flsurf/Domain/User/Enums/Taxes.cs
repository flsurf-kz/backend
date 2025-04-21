namespace Flsurf.Domain.User.Enums
{
    public enum LegalStatus
    {
        Individual,
        SoleProprietor,
        Entity
    }

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
