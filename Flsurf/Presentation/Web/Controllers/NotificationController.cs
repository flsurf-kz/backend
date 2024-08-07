﻿using Flsurf.Application.Common.Models;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.User.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUserService _userService;

        public NotificationController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<NotificationController>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ICollection<NotificationEntity>>> GetNotifications(Guid userId, [FromQuery] InputPagination pagination)
        {
            return Ok(await _userService.GetNotifications().Execute(
                new GetNotificationsDto()
                {
                    UserId = userId,
                    Start = pagination.Start,
                    Ends = pagination.Ends
                }));
        }

        // POST api/<NotificationController>
        [HttpPost("")]
        public async Task<ActionResult<NotificationCreatedDto>> CreateNotification([FromBody] CreateNotificationDto model)
        {
            var result = await _userService.CreateNotification().Execute(model);
            return Ok(result);
        }
    }
}
