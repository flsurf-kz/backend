using Flsurf.Domain.Payment.Enums;

namespace Flsurf.Application.Freelance.Queries
{
    public class FinanceSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Currency { get; set; } = "KZT";
        public DateTime GeneratedAt { get; set; }

        public TotalsDto TotalEarn { get; set; } = null!;
        public List<ContractSummaryDto> TopContracts { get; set; } = [];
        public List<ActivitySummaryDto> TopActivities { get; set; } = [];
        public EarningsSectionDto Earnings { get; set; } = null!;
        public List<DailyPointDto> DailyBreakdown { get; set; } = [];

        public decimal AvgHourlyRate { get; set; }
        public int DaysWorked { get; set; }
        public DateTime? BestDay { get; set; }
        public decimal BestDayAmount { get; set; }
    }

    public class TotalsDto
    {
        public decimal HourlyAmount { get; set; }
        public decimal ManualAmount { get; set; }
        public decimal FixedAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal TaxWithheld { get; set; }
        public decimal NetAmount { get; set; }
    }

    public class ContractSummaryDto
    {
        public Guid ContractId { get; set; }
        public string ContractLabel { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class ActivitySummaryDto   // пока заглушка — см. комментарий в коде
    {
        public Guid ActivityId { get; set; }
        public string Description { get; set; } = null!;
        public int Count { get; set; }
    }

    public class FixedEarnDto
    {
        public Guid ContractId { get; set; }
        public string ContractLabel { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class WorkSessionSummaryDto
    {
        public Guid SessionId { get; set; }
        public Guid ContractId { get; set; }
        public string ContractLabel { get; set; } = null!;
        public string Comment { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public decimal Amount { get; set; }
        public bool IsManual { get; set; }

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
        public List<FixedEarnDto> Fixed { get; set; } = [];
        public List<WorkSessionSummaryDto> Hourly { get; set; } = [];
    }

    public class DailyPointDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
