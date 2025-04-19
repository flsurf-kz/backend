using Flsurf.Application.Common.cqrs;
using Flsurf.Domain.Freelance.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetWorkSessionListQuery : BaseQuery
    {
        [Required]
        public required Guid ContractId { get; set; } // ID контракта, снэпшоты которого нужно получить
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10; 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public WorkSessionStatus? Status { get; set; }
    }
}
