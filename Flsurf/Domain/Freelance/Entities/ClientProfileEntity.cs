using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ClientProfileEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyDescription { get; set; } = string.Empty;
        public string? CompanyWebsite { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public FileEntity? CompanyLogo { get; set; }
        public ClientType ClientType { get; set; }
        
        // Номер 
        public bool IsPhoneVerified { get; set; } = false; 
        public string? PhoneNumber { get; set; } = string.Empty;

        // Работы 
        public ICollection<JobEntity> Jobs { get; set; } = []; 
        public ICollection<ContractEntity> Contracts { get; set; } = [];

        // Последняя активность
        public DateTime? LastActiveAt { get; set; }

        public bool Suspended { get; set; } = false;
    }
}
