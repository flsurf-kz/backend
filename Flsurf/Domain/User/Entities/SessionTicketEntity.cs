using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.User.Entities
{
    public class SessionTicketEntity 
    {
        [Key, Required]
        public Guid Id { get; set; } 
        [Required]
        public byte[] Value { get; set; } = null!;
        [Required]
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
