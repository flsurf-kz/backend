using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Presentation.Web.ExceptionHandlers;
using Flsurf.Presentation.Web.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flsurf.Presentation.Web.Controllers
{
    [SwaggerTag("auth")]
    [Route("api/auth/")]
    [ApiController]
    [TypeFilter(typeof(GuardClauseExceptionFilter))]
    public class AuthControllers : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthControllers(
            IConfiguration configuration,
            IUserService userService
        )
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserEntity>> RegisterUser([FromBody] CreateUserScheme model)
        {
            var user = await _userService
                .Create()
                .Execute(
                    new CreateUserDto()
                    {
                        EmailAddress = model.Email,
                        Password = model.Password,
                        Name = model.Name
                    }
                );

            return Ok(user);
        }

        // sends code to particular email 
        [HttpPost("reset/code/{email}")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            await _userService
                .SendResetPasswordCode()
                .Execute(new SendResetCodeDto() { Email = email });

            return Ok(true);
        }

        [HttpPost("reset/code/verify/{code}")]
        public async Task<IActionResult> VerifyCode(
            string code,
            [FromBody] ResetPasswordScheme model)
        {
            await _userService
                .ResetPassword()
                .Execute(
                    new ResetPasswordDto()
                    {
                        Code = code,
                        NewPassword = model.NewPassword,
                        Email = model.Email
                    });

            return Ok(true);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseSchema>> Login([FromBody] LoginUserSchema model)
        {
            var user = await _userService
                .Get()
                .Execute(new GetUserDto() { Email = model.Email });

            if (user == null)
            {
                // Handle invalid user
                return Unauthorized();
            }

            // Генерируем JWT токен
            var token = GenerateJwtToken(user.Id, user.Fullname); // Pass user ID and username
            var expiresIn = _configuration["Jwt:ExpireMinutes"];
            if (expiresIn == null)
            {
                throw new Exception("Jwt:ExpireMinutes is empty");
            }

            // Возвращаем Bearer JWT токен
            return Ok(new LoginResponseSchema { AccessToken = token, ExpiresIn = expiresIn });
        }


        private string GenerateJwtToken(Guid userId, string username)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("Jwt:SecretKey is empty");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Add user ID to claims
                new Claim(ClaimTypes.Name, username) // Optionally, add username to claims
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
