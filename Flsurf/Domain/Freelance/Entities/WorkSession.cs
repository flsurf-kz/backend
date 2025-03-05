using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class WorkSessionEntity : BaseAuditableEntity
    {
        public ICollection<WorkSnapshotEntity> Snapshots { get; set; } = []; 
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; } = null!;
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Comment { get; set; } = string.Empty;


        public string? AdminComment { get; set; } // Комментарий от модератора, если потребуется

        public int WorkedHours()
        {
            if (EndDate == null)
                return 0;

            return StartDate.Hour - EndDate.Value.Hour;
        }

        public decimal TotalEarned()
        {
            if (Contract == null)
                throw new NullReferenceException("Contract is not loaded dumbass");

            if (Contract.BudgetType != BudgetType.Hourly || Contract.CostPerHour == null)
                return -1;

            return Contract.CostPerHour.Value * WorkedHours();
        }

        public void StartSession()
        {
            Status = WorkSessionStatus.InProgress;

            AddDomainEvent(new WorkSessionStarted(this));
        }
    }
}
