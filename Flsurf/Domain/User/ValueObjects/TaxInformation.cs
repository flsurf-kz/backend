using Microsoft.EntityFrameworkCore;

namespace Flsurf.Domain.User.ValueObjects
{
    [Owned]                              // EF Core 8
    public record TaxInformation(
        string LegalName,
        string TaxId,
        string Country,
        string VatNumber,
        DateOnly? VatValidTo);
}
