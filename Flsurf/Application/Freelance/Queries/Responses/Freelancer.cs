namespace Flsurf.Application.Freelance.Queries.Responses
{
    public class FreelancerStatsDto
    {
        public decimal EarningsLast12Months { get; set; }
        public int JobSuccessScore { get; set; }       // 0–100
        public List<ProfileViewDto> ProfileViews { get; set; } = new();
        public ProposalsDto Proposals { get; set; } = new();
        public int LongTermClients { get; set; }       // > 90 дней
        public int ShortTermClients { get; set; }      // ≤ 90 дней
    }

    public class ProfileViewDto
    {
        public string Date { get; set; } = string.Empty;  // "yyyy-MM-dd"
        public int Count { get; set; }
    }

    public class ProposalsDto
    {
        public int Sent { get; set; }
        public int Viewed { get; set; }
        public int Hires { get; set; }
    }
}
