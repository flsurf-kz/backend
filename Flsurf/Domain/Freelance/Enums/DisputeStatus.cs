namespace Flsurf.Domain.Freelance.Enums
{
    public enum DisputeStatus
    {
        None = 0,         // Спора нет
        Opened,           // Спор открыт
        UnderReview,      // Спор рассматривается
        Resolved,         // Спор решен
        EscalatedToAdmin, // Спор передан администратору
        Closed            // Спор закрыт
    }
}
