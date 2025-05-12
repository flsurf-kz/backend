using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Models;
using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetJobsListQuery : BaseQuery
    {
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10;

        public string? Search { get; set; } 
        public Guid? CategoryId { get; set; } 
        public JobLevel[]? Levels { get; set; } 

        public bool? IsHourly { get; set; } 
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int? MinHourlyRate { get; set; }
        public int? MaxHourlyRate { get; set; }

        public int? MinProposals { get; set; } 
        public int? MaxProposals { get; set; } 

        public int? MinDurationDays { get; set; }
        public int? MaxDurationDays { get; set; }

        public Countries? EmployerLocation { get; set; } 
        public JobStatus[]? Statuses { get; set; } 
        public SortByTypes? SortType { get; set; }
        public SortOption? SortOption { get; set; }

        public Guid? ClientId { get; set; }
        public Guid? FreelancerId { get; set; }

        public bool? Recommended { get; set; }
    }

}
