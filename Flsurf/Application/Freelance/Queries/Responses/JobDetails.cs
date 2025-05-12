using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Application.Freelance.Queries.Responses
{
    public class JobDetails
    {
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public Money? Budget { get; set; } = null!; 
        public CurrencyEnum Currency { get; set; }

        public string Category { get; set; } = null!;
        public string CategorySlug { get; set; } = string.Empty;    
        public string[] Skills { get; set; } = Array.Empty<string>();
        public string[] Languages { get; set; } = Array.Empty<string>();

        public string ClientName { get; set; } = string.Empty;
        public string ClientAvatarUrl { get; set; } = string.Empty;
        public bool IsClientVerified { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? Deadline { get; set; }

        // 🔥 **Активность на заказе**
        public int ResponsesRangeMin { get; set; }
        public int ResponsesRangeMax { get; set; }
        public int DailyResponsesMin { get; set; }
        public int DailyResponsesMax { get; set; }
        public int ConfirmedResponses { get; set; }

        public int Views { get; set; }
    }

}
