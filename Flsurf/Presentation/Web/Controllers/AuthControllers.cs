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
using Microsoft.AspNetCore.Identity;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [SwaggerTag("auth")]
    [Route("api/auth/")]
    [TypeFilter(typeof(GuardClauseExceptionFilter))]
    public class AuthController(IUserService _userService, PasswordService passwordService) : ControllerBase
    {
        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<CommandResult>> Login([FromBody] LoginUserSchema model)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return CommandResult.Forbidden("Вы уже авторизованы").MapResult(this); 
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

            var rememberFor = 60; 
            if (model.RememberMe)
            {
                rememberFor = 60 * 24 * 7; 
            }
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(rememberFor)
                });

            return CommandResult.Success("Authenticated", user.Id).MapResult(this); 
        }

        [HttpPost("logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("register", Name = "Register")]
        public async Task<ActionResult<CommandResult>> Register([FromBody] RegisterUserSchema model)
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
                UserType = model.Type,
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
            
            var rememberFor = 60;
            if (model.RememberMe)
            {
                rememberFor = 60 * 24 * 7;
            }
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(rememberFor)
                });

            return CommandResult.Success(user.Id).MapResult(this);
        }

        // В AuthController.cs
        [HttpGet("external-login/{provider}", Name = "ExternalLogin")]
        public IActionResult ExternalLogin(string provider, [FromQuery] string? returnUrl) // Получаем returnUrl из query
        {
            // Валидация returnUrl (ОЧЕНЬ ВАЖНО для безопасности - предотвращение Open Redirect)
            string validatedReturnUrl = "http://localhost:5173/"; // URL по умолчанию
            if (!string.IsNullOrEmpty(returnUrl) &&
                Uri.TryCreate(returnUrl, UriKind.Absolute, out var uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) &&
                uriResult.Host == "localhost" && // Для разработки - localhost
                uriResult.Port == 5173)
            // В продакшене: (uriResult.Host == "yourfrontend.com")
            {
                validatedReturnUrl = returnUrl;
            }

            var authProperties = new AuthenticationProperties
            {
                // RedirectUri здесь - это куда OIDC провайдер вернет пользователя ПОСЛЕ ЕГО аутентификации.
                // Это должен быть ваш ExternalLoginCallback на бэкенде.
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)), // Имя вашего callback метода
                Items = { { "LoginRedirectUrl", validatedReturnUrl } } // Сохраняем URL фронтенда для финального редиректа
            };

            return Challenge(authProperties, provider);
        }

        // В AuthController.cs

        [HttpGet("external-login-callback", Name = "ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            /* 1. Читаем временную cookie, куда OIDC-провайдер (Google) положил claims.
             *    Схема указана в AddGoogleOpenIdConnect(…). */
            var extResult = await HttpContext.AuthenticateAsync("External");
            if (!extResult.Succeeded || extResult.Principal == null)
                return BadRequest("Ошибка при загрузке данных внешнего входа.");

            /* 2. Берём данные пользователя от провайдера */
            var extPrincipal = extResult.Principal;
            var email = extPrincipal.FindFirstValue(ClaimTypes.Email);
            var fullNameRaw = extPrincipal.FindFirstValue(ClaimTypes.Name);
            var givenName = extPrincipal.FindFirstValue(ClaimTypes.GivenName);
            var surname = extPrincipal.FindFirstValue(ClaimTypes.Surname);
            var avatarUrl = extPrincipal.FindFirstValue("picture");

            if (string.IsNullOrEmpty(email))
                return BadRequest("Провайдер не вернул email.");

            string fullName = fullNameRaw ??
                              $"{givenName} {surname}".Trim() ??
                              email.Split('@')[0];

            /* 3. Создаём / ищем пользователя в нашей БД */
            var externalDto = new ExternalUserDto
            {
                Email = email,
                FullName = fullName,
                Provider = extResult.Properties?.Items[".AuthScheme"] ?? "Google",
                AvatarUrl = avatarUrl ?? ""
            };

            UserEntity user = await _userService
                .FindOrCreateExternalUser()
                .Execute(externalDto);

            /* 4. Чистим временную cookie (схема "External") */
            await HttpContext.SignOutAsync("External");

            /* 5. Формируем ClaimsPrincipal приложения и логиним пользователя */
            var appPrincipal = CreatePrincipal(user);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                appPrincipal,
                new AuthenticationProperties {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1), 
                    IsPersistent = true 
                }
            );

            /* 6. Берём сохранённый URL для редиректа на фронт */
            string redirect =
                extResult.Properties?.Items["LoginRedirectUrl"]
                ?? "http://localhost:5173/";

            // Базовая валидация, чтобы не было open-redirect
            if (!Uri.TryCreate(redirect, UriKind.Absolute, out var uri) ||
                (uri.Host != "localhost" || uri.Port != 5173))
                redirect = "http://localhost:5173/";

            return Redirect(redirect);
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

            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme // Тип аутентификации
            );

            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Отправляет на указанный email код для сброса пароля
        /// </summary>
        [HttpPost("send-reset-code", Name = "SendResetPasswordCode")]
        public async Task<ActionResult<CommandResult>> SendResetPasswordCode([FromBody] SendResetCodeCommand model)
        {
            // Если пользователь уже авторизован, регистрация не разрешается
            if (User?.Identity?.IsAuthenticated == true)
            {
                return CommandResult.BadRequest("Вы уже авторизованы, сброс пароля невозможен").MapResult(this);
            }

            // Формируем команду для отправки кода сброса
            var command = new SendResetCodeCommand
            {
                Email = model.Email
            };

            // Выполняем команду через UserService
            var result = await _userService.SendResetPasswordCode().Handle(command);

            if (!result.IsSuccess)
                return result.MapResult(this);

            return CommandResult.Success().MapResult(this);
        }

        /// <summary>
        /// Сбрасывает пароль пользователя по коду, отправленному на email
        /// </summary>
        [HttpPost("reset-password", Name = "ResetPassword")]
        public async Task<ActionResult<CommandResult>> ResetPassword([FromBody] ResetPasswordCommand model)
        {
            // Формируем команду для сброса пароля
            var command = new ResetPasswordCommand
            {
                Code = model.Code,
                NewPassword = model.NewPassword
            };

            var result = await _userService.ResetPassword().Handle(command);

            if (!result.IsSuccess)
                return result.MapResult(this);

            return CommandResult.Success().MapResult(this);
        }

        [HttpPatch("security-answer", Name = "AddSecurityAnswer")]
        public async Task<ActionResult<CommandResult>> AddSecurityAnswer([FromBody] AddSecurityQuestionCommand cmd)
        {
            return await _userService.AddSecurityQuestion().Handle(cmd); 
        }

        [HttpPost("security-answer", Name = "AuthWithSecurityAnswer")]
        public async Task<ActionResult<SecurityAnswerDto?>> AuthiticateWithSecurityAnswer([FromBody] AuthticateWithSecurityAnswerQuery query)
        {
            var result = await _userService.AuthticateWithSecretAnswer().Handle(query);
            if (result == null)
                return NoContent(); 
            return result; 
        }
    }
}
