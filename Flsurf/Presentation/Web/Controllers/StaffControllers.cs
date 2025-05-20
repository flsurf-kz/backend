using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Interfaces;
using Flsurf.Application.User.Commands;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Presentation.Web.ExceptionHandlers;
using Flsurf.Presentation.Web.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/stuff/")]
    [ApiController]
    [TypeFilter(typeof(GuardClauseExceptionFilter))]
    public class StaffControllers : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStaffService _staffService;

        public StaffControllers(IUserService userService, IStaffService staffService)
        {
            _userService = userService;
            _staffService = staffService;
        }

        [HttpPost("user/{userId}/block", Name = "BlockUser")]
        public async Task<ActionResult<bool>> BlockUser(Guid userId)
        {
            await _userService.BlockUser()
                .Handle(new BlockUserCommand { UserId = userId, Blocked = true });
            return Ok(true);
        }

        [HttpPost("user/{userId}/warn", Name = "WarnUser")]
        public async Task<ActionResult<bool>> WarnUser(Guid userId, [FromBody] WarnUserScheme model)
        {
            await _userService.WarnUser()
                .Handle(new WarnUserCommand() { UserId = userId, Reason = model.Reason });
            return Ok(true);
        }

        [HttpPost("ticket", Name = "CreateTicket")]
        public async Task<ActionResult<TicketEntity>> CreateTicket([FromBody] CreateTicketDto ticket)
        {
            return Ok(await _staffService.CreateTicket().Execute(ticket));
        }

        [HttpPost("ticket/search", Name = "GetTickets")]
        public async Task<ActionResult<ICollection<TicketEntity>>> GetTickets([FromBody] GetTicketsDto model)
        {
            return Ok(await _staffService.GetTicketsList().Execute(model));
        }

        [HttpGet("ticket/{ticketId}", Name = "GetTicket")]
        public async Task<ActionResult<TicketEntity>> GetTicket(Guid ticketId)
        {
            return Ok(await _staffService.GetTicket().Execute(new GetTicketDto() { TicketId = ticketId }));
        }

        // ------------- CREATE ------------------------------------------------
        [HttpPost(Name = "CreateNews")]
        public async Task<ActionResult<NewsEntity>> CreateNews([FromBody] CreateNewsDto dto)
        {
            var news = await _staffService.CreateNews().Execute(dto);
            return CreatedAtRoute("GetNewsById", new { newsId = news.Id }, news);
        }

        // ------------- READ – список ----------------------------------------
        [HttpGet(Name = "GetNewsList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<NewsEntity>>> GetNewsList([FromQuery] GetNewsListDto dto)
        {
            var list = await _staffService.GetNewsList().Execute(dto);
            return Ok(list);
        }

        // ------------- READ – по Id -----------------------------------------
        [HttpGet("{newsId:guid}", Name = "GetNewsById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NewsEntity>> GetNewsById(Guid newsId)
        {
            var news = await _staffService.GetNews().Execute(newsId);
            return news is null ? NotFound() : Ok(news);
        }

        // ------------- UPDATE ------------------------------------------------
        [HttpPut("{newsId:guid}", Name = "UpdateNews")]
        public async Task<ActionResult<NewsEntity>> UpdateNews(Guid newsId, [FromBody] UpdateNewsDto dto)
        {
            dto.NewsId = newsId;
            var updated = await _staffService.UpdateNews().Execute(dto);
            return Ok(updated);
        }

        // ------------- DELETE ------------------------------------------------
        [HttpDelete("{newsId:guid}", Name = "DeleteNews")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteNews(Guid newsId)
        {
            var removed = await _staffService.DeleteNews().Execute(newsId);
            return removed ? NoContent() : NotFound();
        }
    }
}
