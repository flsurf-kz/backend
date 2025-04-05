using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.WorkSession;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/work-session")]
    [Authorize]
    public class WorkSessionController : ControllerBase
    {
        private readonly IWorkSessionService _workSessionService;

        public WorkSessionController(IWorkSessionService workSessionService)
        {
            _workSessionService = workSessionService;
        }

        [HttpPost("start", Name = "StartSession")]
        public async Task<ActionResult<CommandResult>> StartSession([FromBody] StartWorkSessionCommand command)
        {
            var handler = _workSessionService.StartWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("submit", Name = "SubmitSession")]
        public async Task<ActionResult<CommandResult>> SubmitSession([FromBody] SubmitWorkSessionCommand command)
        {
            var handler = _workSessionService.SubmitWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("end", Name = "EndSession")]
        public async Task<ActionResult<CommandResult>> EndSession([FromBody] EndWorkSessionCommand command)
        {
            var handler = _workSessionService.EndWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("approve", Name = "ApproveSession")]
        public async Task<ActionResult<CommandResult>> ApproveSession([FromBody] ApproveWorkSessionCommand command)
        {
            var handler = _workSessionService.ApproveWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("react", Name = "ReactSession")]
        public async Task<ActionResult<CommandResult>> ReactSession([FromBody] ReactToWorkSessionCommand command)
        {
            var handler = _workSessionService.ReactToWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}", Name = "GetSession")]
        public async Task<ActionResult<WorkSessionEntity>> GetSession(Guid id)
        {
            var query = new GetWorkSessionQuery { WorkSessionId = id };
            var handler = _workSessionService.GetWorkSession();
            var session = await handler.Handle(query);
            if (session == null)
                return NotFound("Рабочая сессия не найдена");
            return Ok(session);
        }

        [HttpGet("list", Name = "GetSessionList")]
        public async Task<ActionResult<ICollection<WorkSessionEntity>>> GetSessionList(
            [FromQuery] Guid contractId,
            [FromQuery] int start = 0,
            [FromQuery] int end = 10
        )
        {
            var query = new GetWorkSessionListQuery()
            {
                ContractId = contractId,
                Start = start,
                Ends = end
            };
            var handler = _workSessionService.GetWorkSessionsList();
            var sessions = await handler.Handle(query);
            return Ok(sessions);
        }
    }
}
