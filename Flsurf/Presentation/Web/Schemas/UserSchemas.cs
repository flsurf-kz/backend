
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Presentation.Web.Schemas
{
    public class UserScheme
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Fullname { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;

        public static UserScheme FromEntity(UserEntity entity)
        {
            return new UserScheme
            {
                Id = entity.Id,
                Fullname = entity.Fullname,
                Email = entity.Email
            };
        }
    }

    public class CreateUserScheme
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
    }

    public class WarnUserScheme
    {
        public string Reason { get; set; } = null!;
    }
}
