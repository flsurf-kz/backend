using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.User.ValueObjects
{
    [Owned]
    public record class BankDetails
    {
        // Для EF Core
        private BankDetails() { }

        // Доменный конструктор
        public BankDetails(
            [Required, MaxLength(20)] string bic,
            [Required, MaxLength(34)] string accountNumber,
            [Required, MaxLength(200)] string bankName)
        {
            Bic = bic;
            AccountNumber = accountNumber;
            BankName = bankName;
        }

        [Required, MaxLength(20)]
        public string Bic { get; init; }

        [Required, MaxLength(34)]
        public string AccountNumber { get; init; }

        [Required, MaxLength(200)]
        public string BankName { get; init; }
    }

    [Owned]
    public record class TaxInformation
    {
        // Для EF Core
        private TaxInformation() { }

        // Доменный конструктор
        public TaxInformation(
            string countryIso,
            string localIdNumber,
            LegalStatus legalStatus,
            TaxRegime taxRegime,
            bool vatRegistered,
            string? vatNumber,
            BankDetails bankDetails)
        {
            CountryIso = countryIso;
            LocalIdNumber = localIdNumber;
            LegalStatus = legalStatus;
            TaxRegime = taxRegime;
            VatRegistered = vatRegistered;
            VatNumber = vatNumber;
            BankDetails = bankDetails;
        }

        [Required, StringLength(2, MinimumLength = 2)]
        public string CountryIso { get; init; }

        [Required, StringLength(12, MinimumLength = 9)]
        public string LocalIdNumber { get; init; }

        [Required]
        public LegalStatus LegalStatus { get; init; }

        [Required]
        public TaxRegime TaxRegime { get; init; }

        public bool VatRegistered { get; init; }

        public string? VatNumber { get; init; }

        [Required]
        public BankDetails BankDetails { get; init; }
    }
}
