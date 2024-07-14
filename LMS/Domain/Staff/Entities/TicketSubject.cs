using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Staff.Entities
{
    public class TicketSubjectEntity : BaseAuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string DefaultText { get; set; } = null!; 

        public static TicketSubjectEntity Create(string Name, string DefaultText)
        {
            return new TicketSubjectEntity() { Name = Name, DefaultText = DefaultText };
        }
    }
}
