using System.ComponentModel.DataAnnotations;

namespace Flsurf.Presentation.Web.Schemas
{
    public class LoginResponseSchema
    {
        [Required]
        public string AccessToken { get; set; } = null!;
        [Required]
        public string ExpiresIn { get; set; } = null!;
    }

    public class ResetPasswordScheme
    {
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
    }

    public class LoginUserSchema
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;
    }
}
