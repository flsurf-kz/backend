using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ClientProfileEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!; 
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string CompanyWebsite { get; set; } = string.Empty;
        public float Rating { get; set; } = 0; 
        public string Location { get; set; } = string.Empty;
        public string CompanyLogo { get; set; } = string.Empty;
        public string EmployerType { get; set; } = string.Empty;
        
        // Номер 
        public bool IsPhoneVerified { get; set; } = false; 
        public string PhoneNumber { get; set; } = string.Empty;

        // Работы 
        public ICollection<JobEntity> Jobs { get; set; } = []; 
        public ICollection<ContractEntity> Contracts { get; set; } = [];

        // Последняя активность
        public DateTime? LastActiveAt { get; set; }

        public bool Suspended { get; set; } = false;
    }
}
