using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContestEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; } = null!; 
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Money PrizePool { get; set; } = null!; 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null!;
        public ContestStatus Status { get; set; } = ContestStatus.Draft; 
        [ForeignKey(nameof(ContestEntryEntity))]
        public Guid? WinnerEntryId { get; set; }
        public bool IsResultPublic { get; set; } = true; 
        public bool IsEntriesPublic { get; set; } = false;
        public ICollection<FileEntity> Files { get; set; } = [];
        [JsonIgnore]
        public ICollection<ContestEntryEntity> ContestEntries { get; set; } = []; 

        public void SelectWinner(ContestEntryEntity entry)
        {
            if (!ContestEntries.Contains(entry))
                throw new DomainException("Нету в списке притендетов такого предложения"); 
            if (Status != ContestStatus.Open)
                throw new DomainException("Конкурс все еще не открыт");
            if (WinnerEntryId != null)
                throw new NullReferenceException("Баг в бизнес логике, победитель уже выбран");

            WinnerEntryId = entry.Id; 
            Status = ContestStatus.WinnerSelected;

            entry.Reaction = ContestEntryReaction.Winner; 
        }
    }
}
