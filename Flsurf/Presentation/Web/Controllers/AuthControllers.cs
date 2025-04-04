using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Presentation.Web.ExceptionHandlers;
using Flsurf.Presentation.Web.Schemas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Flsurf.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [SwaggerTag("auth")]
    [Route("api/auth/")]
    [TypeFilter(typeof(GuardClauseExceptionFilter))]
    public class AuthController(IUserService _userService, PasswordService passwordService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserSchema model)
        {
            var user = await _userService.Get().Execute(new GetUserDto { Email = model.Email });
            if (user == null || !user.VerifyPassword(model.Password, passwordService))
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Fullname)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                });

            return Ok(new { Message = "Authenticated" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpGet("external-login/{provider}")]
        public IActionResult ExternalLogin(string provider)
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback")
            };

            return Challenge(authProperties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
                return BadRequest("Error loading external login");

            var externalUser = result.Principal;

            var userDto = new ExternalUserDto
            {
                Email = externalUser.FindFirstValue(ClaimTypes.Email),
                FullName = externalUser.FindFirstValue(ClaimTypes.Name),
                Provider = result.Properties.Items[".AuthScheme"] // Имя провайдера
            };

            var user = await _userService
                .FindOrCreateExternalUser()
                .Execute(userDto);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                CreatePrincipal(user));

            return Redirect("/");
        }

        private ClaimsPrincipal CreatePrincipal(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim("IsExternalUser", user.IsExternalUser.ToString()),
        
                // Добавьте кастомные claims при необходимости
                new Claim("AvatarUrl", user.Avatar?.FilePath ?? string.Empty),
                new Claim("UserType", user.Type.ToString())
            };

            // Добавление ролей (если используется Role-Based Access Control)
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme // Тип аутентификации
            );

            return new ClaimsPrincipal(identity);
        }
    }

}
