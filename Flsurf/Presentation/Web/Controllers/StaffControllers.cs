﻿using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Presentation.Web.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/stuff/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class StaffControllers : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStaffService _staffService;

        public StaffControllers(IUserService userService, IStaffService staffService)
        {
            _userService = userService;
            _staffService = staffService;
        }

        [HttpPost("user/{userId}/block")]
        public async Task<ActionResult<bool>> BlockUser(Guid userId)
        {
            await _userService.Update()
                .Execute(new UpdateUserDto { UserId = userId, Blocked = true });
            return Ok(true);
        }

        [HttpPost("user/{userId}/warn")]
        public async Task<ActionResult<bool>> WarnUser(Guid userId, [FromBody] WarnUserScheme model)
        {
            await _userService.WarnUser()
                .Execute(new WarnUserDto() { UserId = userId, Reason = model.Reason });
            return Ok(true);
        }

        [HttpPost("ticket")]
        public async Task<ActionResult<TicketEntity>> CreateTicket([FromBody] CreateTicketDto ticket)
        {
            return Ok(await _staffService.CreateTicket().Execute(ticket));
        }

        [HttpGet("ticket")]
        public async Task<ActionResult<ICollection<TicketEntity>>> GetTickets([FromBody] GetTicketsDto model)
        {
            return Ok(await _staffService.GetTicketsList().Execute(model));
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<ActionResult<TicketEntity>> GetTicket(Guid ticketId)
        {
            return Ok(await _staffService.GetTicket().Execute(new GetTicketDto() { TicketId = ticketId }));
        }
    }
}
