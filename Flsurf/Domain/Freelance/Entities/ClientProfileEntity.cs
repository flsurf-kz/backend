using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ClientProfileEntity : BaseAuditableEntity
    {
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyWebsite { get; set; }
        public float Rating { get; set; }
        public string Location { get; set; }
        public string CompanyLogo { get; set; }
        public string EmployerType { get; set; }
    }
}
