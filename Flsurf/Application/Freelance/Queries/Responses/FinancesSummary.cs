using Flsurf.Domain.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Queries
{
    public class FinanceSummaryDto
    {
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Currency { get; set; } = "KZT";
        [Required]
        public DateTime GeneratedAt { get; set; }

        [Required]
        public TotalsDto TotalEarn { get; set; } = null!;
        [Required]
        public List<ContractSummaryDto> TopContracts { get; set; } = [];
        [Required]
        public List<ActivitySummaryDto> TopActivities { get; set; } = [];
        [Required]
        public EarningsSectionDto Earnings { get; set; } = null!;
        [Required]
        public List<DailyPointDto> DailyBreakdown { get; set; } = [];

        [Required]
        public decimal AvgHourlyRate { get; set; }
        [Required]
        public int DaysWorked { get; set; }
        public DateTime? BestDay { get; set; }
        [Required]
        public decimal BestDayAmount { get; set; }
    }

    public class TotalsDto
    {
        [Required]
        public decimal HourlyAmount { get; set; }
        [Required]
        public decimal ManualAmount { get; set; }
        [Required]
        public decimal FixedAmount { get; set; }
        [Required]
        public decimal PlatformFee { get; set; }
        [Required]
        public decimal TaxWithheld { get; set; }
        [Required]
        public decimal NetAmount { get; set; }
    }

    public class ContractSummaryDto
    {
        [Required]
        public Guid ContractId { get; set; }
        [Required]
        public string ContractLabel { get; set; } = null!;
        [Required]
        public decimal Amount { get; set; }
    }

    public class ActivitySummaryDto   // пока заглушка — см. комментарий в коде
    {
        [Required]
        public Guid ActivityId { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public int Count { get; set; }
    }

    public class FixedEarnDto
    {
        [Required]
        public Guid ContractId { get; set; }
        [Required]
        public string ContractLabel { get; set; } = null!;
        [Required]
        public decimal Amount { get; set; }
    }

    public class WorkSessionSummaryDto
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public Guid ContractId { get; set; }
        [Required]
        public string ContractLabel { get; set; } = null!;
        [Required]
        public string Comment { get; set; } = string.Empty;
        [Required]
        public decimal Hours { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool IsManual { get; set; }

        [Required]
        public DateTime StartUtc { get; set; }    // <‑ новoе
        public DateTime? EndUtc { get; set; }    // <‑ новoе (не null)

        public IEnumerable<DateOnly> GetWorkDates()
        {
            if (EndUtc is null)
            {
                yield break;
            }

            var current = StartUtc.Date;
            var last = EndUtc.Value.Date;

            while (current <= last)
            {
                yield return DateOnly.FromDateTime(current);
                current = current.AddDays(1);
            }
        }
    }

    public class EarningsSectionDto
    {
        [Required]
        public List<FixedEarnDto> Fixed { get; set; } = [];
        [Required]
        public List<WorkSessionSummaryDto> Hourly { get; set; } = [];
    }

    public class DailyPointDto
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
