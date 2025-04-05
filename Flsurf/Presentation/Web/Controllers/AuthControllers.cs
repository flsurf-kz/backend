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
using System.Xml.Linq;
using Flsurf.Application.User.Queries;
using Flsurf.Application.User.Commands;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Common.cqrs;

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
            if (User?.Identity?.IsAuthenticated == true)
            {
                return BadRequest("Вы уже авторизованы. Для входа пользователя, пожалуйста, выйдите из системы.");
            }
            var user = await _userService.GetUser().Handle(new GetUserQuery { Email = model.Email });
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserSchema model)
        {
            // Если пользователь уже авторизован, регистрация не разрешается
            if (User?.Identity?.IsAuthenticated == true)
            {
                return CommandResult.BadRequest("Вы уже авторизованы, вам нельзя регистрироваться еще раз").MapResult(this);
            }

            // Создаем команду для регистрации пользователя.
            var command = new CreateUserCommand
            {
                Email = model.Email,
                Password = model.Password,
                Name = model.Name,
                Surname = model.Surname,
                UserType = Domain.User.Enums.UserTypes.NonUser, 
            };

            // Выполняем команду создания пользователя через UserService.
            var result = await _userService.CreateUser().Handle(command);

            if (!result.IsSuccess)
                return result.MapResult(this); 

            // it should be fast
            var user = await _userService.GetUser().Handle(new GetUserQuery() { UserId = result.GetIdAsGuid() });

            // it is just impossible
            if (user == null)
                return result.MapResult(this); 

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
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                });

            return CommandResult.Success(user.Id).MapResult(this);
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
