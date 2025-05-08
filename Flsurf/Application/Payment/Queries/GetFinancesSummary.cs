using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Queries.Models;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Queries
{
    public class GetFinanceSummaryQuery : BaseQuery
    {
        public int Month { get; }
        public int Year { get; }
        public Guid? UserId { get; }
    }


    public sealed class GetFinanceSummaryHandler(
            IApplicationDbContext db,
            IPermissionService perm)
        : IQueryHandler<GetFinanceSummaryQuery, FinanceSummaryDto>
    {
        private readonly IApplicationDbContext _db = db;
        private readonly IPermissionService _ps = perm;

        public async Task<FinanceSummaryDto> Handle(GetFinanceSummaryQuery q)
        {
            /* ── период ───────────────────────────── */
            var (start, end) = GetMonthBounds(q.Month, q.Year);

            /* ── пользователь и его кошельки ──────── */
            Guid userId = q.UserId ?? (await _ps.GetCurrentUser()).Id;
            var wallets = await _db.Wallets
                                     .Include(w => w.Transactions)
                                     .Where(w => w.UserId == userId)
                                     .ToListAsync();

            /* ── 1. ЧАСЫ (WorkSessions) ───────────── */
            var sessions = await _db.WorkSessions
                .Include(ws => ws.Contract)
                .Where(ws => ws.FreelancerId == userId &&
                             ws.StartDate >= start &&
                             ws.StartDate < end &&
                             ws.EndDate != null)           // закончена
                .ToListAsync();

            var hourly = sessions.Select(ws => new WorkSessionSummaryDto
            {
                SessionId = ws.Id,
                ContractId = ws.ContractId,
                ContractLabel = $"#{ws.ContractId.ToString()[..8]} {ws.Contract.BudgetType}",
                Comment = ws.Comment ?? string.Empty,
                Hours = (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours,
                Amount = ws.TotalEarned().Amount,
                IsManual = ws.Status == WorkSessionStatus.Pending, 
                StartUtc = ws.StartDate, 
                EndUtc = ws.EndDate, 
            }).ToList();

            /* ── 2. ТРАНЗАКЦИИ (через Wallet) ─────── */
            // берём входящие Completed и плюсуем комиссию
            var txs = wallets
                .SelectMany(w => w.Transactions)
                .Where(t => t.Status == TransactionStatus.Completed &&
                            t.Flow == TransactionFlow.Incoming &&
                            t.CompletedAt >= start && t.CompletedAt < end)
                .ToList();

            var fixedEarn = txs.Where(t => t.Type is TransactionType.Bonus or TransactionType.Deposit)
                               .GroupBy(t => t.Type)
                               .Select(g => new FixedEarnDto
                               {
                                   ContractId = Guid.Empty,              // нет связи с контрактом
                                   ContractLabel = g.Key.ToString(),        // "Bonus", "Deposit"
                                   Amount = g.Sum(t => t.NetAmount.Amount)
                               })
                               .ToList();

            /* ── TOP‑5 контрактов (только из WorkSessions) ── */
            var topContracts = hourly.GroupBy(h => new { h.ContractId, h.ContractLabel })
                                     .Select(g => new ContractSummaryDto
                                     {
                                         ContractId = g.Key.ContractId,
                                         ContractLabel = g.Key.ContractLabel,
                                         Amount = g.Sum(x => x.Amount)
                                     })
                                     .OrderByDescending(c => c.Amount)
                                     .Take(5)
                                     .ToList();

            /* ── TOP‑5 активностей — пустой список (нет сущности) ── */
            var topActs = new List<ActivitySummaryDto>();

            /* ── DAILY BREAKDOWN (sessions + tx) ── */
            var daily = BuildDaily(hourly, txs, start);

            /* ── TOTALS ── */
            decimal hourlyTotal = hourly.Sum(h => h.Amount);
            decimal manualTotal = hourly.Where(h => h.IsManual).Sum(h => h.Amount);
            decimal fixedTotal = fixedEarn.Sum(f => f.Amount);
            decimal feeTotal = txs.Sum(t => t.AppliedFee.Amount);
            decimal tax = (hourlyTotal + fixedTotal - feeTotal) * 0.05m;
            decimal net = hourlyTotal + fixedTotal - feeTotal - tax;

            return new FinanceSummaryDto
            {
                Month = q.Month,
                Year = q.Year,
                Currency = wallets.First().Currency.ToString(),   // все кошельки в одной валюте
                GeneratedAt = DateTime.UtcNow,

                TotalEarn = new TotalsDto
                {
                    HourlyAmount = hourlyTotal,
                    ManualAmount = manualTotal,
                    FixedAmount = fixedTotal,
                    PlatformFee = feeTotal,
                    TaxWithheld = tax,
                    NetAmount = net
                },

                AvgHourlyRate = hourly.Any() ? hourlyTotal / hourly.Sum(h => h.Hours) : 0,
                DaysWorked = hourly.SelectMany(h => h.GetWorkDates()).Distinct().Count(),
                BestDay = daily.MaxBy(d => d.Amount)?.Date,
                BestDayAmount = daily.MaxBy(d => d.Amount)?.Amount ?? 0,

                TopContracts = topContracts,
                TopActivities = topActs,

                Earnings = new EarningsSectionDto
                {
                    Hourly = hourly,
                    Fixed = fixedEarn
                },
                DailyBreakdown = daily
            };
        }

        /* ───── helpers ───────────────────────────────────────────────── */

        private static (DateTime start, DateTime end) GetMonthBounds(int m, int y)
        {
            var s = new DateTime(y, m, 1, 0, 0, 0, DateTimeKind.Utc);
            return (s, s.AddMonths(1));
        }

        private static List<DailyPointDto> BuildDaily(
            IEnumerable<WorkSessionSummaryDto> sessions,
            IEnumerable<TransactionEntity> txs,
            DateTime monthStart)
        {
            var dict = new Dictionary<DateOnly, decimal>();

            foreach (var h in sessions)
                foreach (var d in h.GetWorkDates())
                    dict[d] = dict.GetValueOrDefault(d) + (h.Amount / h.Hours);

            foreach (var t in txs)
            {
                var d = DateOnly.FromDateTime(t.CompletedAt!.Value.Date);
                dict[d] = dict.GetValueOrDefault(d) + t.NetAmount.Amount;
            }

            return dict
                .Select(kv => new DailyPointDto
                {
                    Date = kv.Key.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                    Amount = kv.Value
                })
                .OrderBy(p => p.Date)
                .ToList();
        }
    }

}
