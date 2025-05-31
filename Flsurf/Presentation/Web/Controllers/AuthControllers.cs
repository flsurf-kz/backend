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
            // 1. Аутентификация с использованием внешней схемы (схемы OIDC провайдера)
            // Эта схема временно хранит информацию от внешнего провайдера (например, в куке).
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
                return BadRequest("Error loading external login");

            var authenticateResult = result;

            if (authenticateResult?.Succeeded != true)
            {
                string failureMessage = authenticateResult?.Failure?.Message ?? "Unknown external authentication error.";
                // Здесь можно добавить логирование ошибки: _logger.LogError("External login failed: {failureMessage}", failureMessage);
                return BadRequest($"Ошибка при загрузке данных внешнего входа: {failureMessage}");
            }

            // 2. Извлечение Claims от внешнего провайдера
            var externalPrincipal = authenticateResult.Principal;
            if (externalPrincipal == null)
            {
                return BadRequest("Не удалось получить данные пользователя от внешнего провайдера.");
            }

            var email = externalPrincipal.FindFirstValue(ClaimTypes.Email);
            var name = externalPrincipal.FindFirstValue(ClaimTypes.Name) ?? ""; // Может быть полным именем
            var givenName = externalPrincipal.FindFirstValue(ClaimTypes.GivenName); // Имя
            var surname = externalPrincipal.FindFirstValue(ClaimTypes.Surname); // Фамилия

            // ---> ВОТ ЗДЕСЬ МЫ ПОЛУЧАЕМ URL АВАТАРА ОТ OIDC ПРОВАЙДЕРА <---
            var pictureClaimValue = externalPrincipal.FindFirstValue("picture"); // Стандартный OIDC claim
                                                                                 // Некоторые провайдеры могут использовать другие claims, например:
                                                                                 // var pictureClaimValue = externalPrincipal.FindFirstValue("urn:google:picture") ?? externalPrincipal.FindFirstValue("picture");

            // Если хотите проверить, что приходит от провайдера, можно раскомментировать:
            // var allClaims = externalPrincipal.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            // throw new Exception("Claims от провайдера: " + string.Join(", ", allClaims));

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email не предоставлен внешним провайдером. Регистрация невозможна.");
            }

            // Формируем полное имя, если оно разделено
            string fullName = name;
            if (string.IsNullOrEmpty(fullName) && (!string.IsNullOrEmpty(givenName) || !string.IsNullOrEmpty(surname)))
            {
                fullName = $"{givenName} {surname}".Trim();
            }
            if (string.IsNullOrEmpty(fullName)) // Если имя все еще пустое, можно использовать часть email или другое значение по умолчанию
            {
                fullName = email.Split('@')[0];
            }

            // 3. Создание DTO для передачи в ваш сервис пользователей
            var userDto = new ExternalUserDto
            {
                Email = email,
                FullName = fullName,
                Provider = authenticateResult.Properties?.Items?[".AuthScheme"] ?? "UnknownProvider",
                AvatarUrl = pictureClaimValue ?? "" // <--- Передаем полученный URL аватара
            };

            // 4. Поиск или создание пользователя в вашей системе
            // Ваш сервис _userService.FindOrCreateExternalUser().Execute(userDto)
            // ДОЛЖЕН ОБНОВИТЬ ИЛИ СОЗДАТЬ UserEntity С ПОЛУЧЕННЫМ AvatarUrl.
            // Например, userEntity.Avatar.FilePath = userDto.AvatarUrl; (или как у вас устроено хранение)
            UserEntity user = await _userService
                .FindOrCreateExternalUser()
                .Execute(userDto);

            if (user == null)
            {
                return BadRequest("Не удалось создать или найти пользователя в системе.");
            }

            // 6. Создание ClaimsPrincipal для вашего приложения, включая AvatarUrl из вашей UserEntity
            // Метод CreatePrincipal должен брать AvatarUrl из user.Avatar.FilePath (который был обновлен на шаге 4)
            var appPrincipal = CreatePrincipal(user);

            // 7. Вход пользователя в ваше приложение (установка основной сессионной куки)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, // Ваша основная схема аутентификации
                appPrincipal,
                new AuthenticationProperties { IsPersistent = true }); // или настройте IsPersistent по вашему желанию

            // Получаем сохраненный URL для редиректа на фронтенд
            string? finalRedirectUrl = authenticateResult.Properties?.Items["LoginRedirectUrl"];

            // Снова валидация URL для безопасности (на случай, если AuthenticationProperties были как-то изменены)
            if (string.IsNullOrEmpty(finalRedirectUrl) ||
                !Uri.TryCreate(finalRedirectUrl, UriKind.Absolute, out var uriResult) ||
                !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) ||
                !(uriResult.Host == "localhost" && uriResult.Port == 5173) /* В продакшене: uriResult.Host != "yourfrontend.com" */ )
            {
                finalRedirectUrl = "http://localhost:5173/"; // URL по умолчанию
            }

            return Redirect(finalRedirectUrl); // Редирект на фронтенд
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
    }
}
