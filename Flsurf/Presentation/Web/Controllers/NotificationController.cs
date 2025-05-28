using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Common.Models;
using Flsurf.Application.User.Commands;
using Flsurf.Application.User.Interfaces;
using Flsurf.Application.User.Queries;
using Flsurf.Domain.User.Entities;
using Flsurf.Presentation.Web.ExceptionHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [TypeFilter(typeof(GuardClauseExceptionFilter))]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IUserService _userService;

        public NotificationController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<NotificationController>
        [HttpGet("user/{userId}", Name = "GetNotifications")]
        public async Task<ActionResult<ICollection<NotificationEntity>>> GetNotifications(Guid userId, [FromQuery] InputPagination pagination)
        {
            return Ok(await _userService.GetNotifications().Handle(
                new GetNotificationsQuery()
                {
                    UserId = userId,
                    Start = pagination.Start,
                    Ends = pagination.Ends
                }));
        }

        // POST api/<NotificationController>
        [HttpPost("", Name = "CreateNotification")]
        public async Task<ActionResult<CommandResult>> CreateNotification([FromBody] CreateNotificationCommand model)
        {
            var result = await _userService.CreateNotifications().Handle(model);
            return result.MapResult(this);
        }

        [HttpPut("hide", Name = "HideNotifications")]
        public async Task<ActionResult<CommandResult>> HideNotifcations([FromBody] HideNotificationsCommand command)
        {
            var result = await _userService.HideNotifications().Handle(command);
            return result.MapResult(this);
        } 
    }
}
