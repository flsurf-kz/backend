using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.ClientProfile;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Application.Freelance.Queries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/client-profile")]
    public class ClientProfileController : ControllerBase
    {
        private readonly IClientProfileService _clientProfileService;

        public ClientProfileController(IClientProfileService clientProfileService)
        {
            _clientProfileService = clientProfileService;
        }

        /// <summary>
        /// Получение информации о заказах клиента.
        /// </summary>
        /// <param name="userId">Идентификатор пользвателя.</param>
        /// <returns>Информация о заказах клиента.</returns>
        [HttpGet("order-info/{userId}")]
        public async Task<ActionResult<ClientJobInfo>> GetClientOrderInfo(Guid userId)
        {
            var query = new GetClientInfoQuery { UserId = userId };
            var handler = _clientProfileService.GetClientOrderInfo();
            var result = await handler.Handle(query);
            if (result == null)
                return NotFound("Информация о заказах не найдена");
            return Ok(result);
        }

        /// <summary>
        /// Создание профиля клиента.
        /// </summary>
        /// <param name="command">Параметры для создания профиля.</param>
        /// <returns>Результат операции создания профиля.</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateClientProfile([FromBody] CreateClientProfileCommand command)
        {
            var handler = _clientProfileService.CreateClientProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        /// <summary>
        /// Приостановка профиля клиента.
        /// </summary>
        /// <param name="command">Параметры для приостановки профиля.</param>
        /// <returns>Результат операции приостановки.</returns>
        [HttpPost("suspend")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> SuspendClientProfile([FromBody] SuspendClientProfileCommand command)
        {
            var handler = _clientProfileService.SuspendClientProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        /// <summary>
        /// Обновление профиля клиента.
        /// </summary>
        /// <param name="command">Параметры для обновления профиля.</param>
        /// <returns>Результат операции обновления профиля.</returns>
        [HttpPost("update")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateClientProfile([FromBody] UpdateClientProfileCommand command)
        {
            var handler = _clientProfileService.UpdateClientProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }
    }
}
