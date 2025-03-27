using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.WorkSession;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/work-session")]
    public class WorkSessionController : ControllerBase
    {
        private readonly IWorkSessionService _workSessionService;

        public WorkSessionController(IWorkSessionService workSessionService)
        {
            _workSessionService = workSessionService;
        }

        [HttpPost("start")]
        public async Task<ActionResult<WorkSessionEntity>> StartSession([FromBody] StartWorkSessionCommand command)
        {
            var handler = _workSessionService.StartWorkSession();
            var result = await handler.Handle(command);
            return MapResult(result);
        }

        [HttpPost("submit")]
        public async Task<ActionResult<WorkSessionEntity>> SubmitSession([FromBody] SubmitWorkSessionCommand command)
        {
            var handler = _workSessionService.SubmitWorkSession();
            var result = await handler.Handle(command);
            return MapResult(result);
        }

        [HttpPost("end")]
        public async Task<ActionResult<WorkSessionEntity>> EndSession([FromBody] EndWorkSessionCommand command)
        {
            var handler = _workSessionService.EndWorkSession();
            var result = await handler.Handle(command);
            return MapResult(result);
        }

        [HttpPost("approve")]
        public async Task<ActionResult<WorkSessionEntity>> ApproveSession([FromBody] ApproveWorkSessionCommand command)
        {
            var handler = _workSessionService.ApproveWorkSession();
            var result = await handler.Handle(command);
            return MapResult(result);
        }

        [HttpPost("react")]
        public async Task<ActionResult<WorkSessionEntity>> ReactSession([FromBody] ReactToWorkSessionCommand command)
        {
            var handler = _workSessionService.ReactToWorkSession();
            var result = await handler.Handle(command);
            return result.MapResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkSessionEntity>> GetSession(Guid id)
        {
            var query = new GetWorkSessionQuery { WorkSessionId = id };
            var handler = _workSessionService.GetWorkSession();
            var session = await handler.Handle(query);
            if (session == null)
                return NotFound("Рабочая сессия не найдена");
            return Ok(session);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<WorkSessionEntity>>> GetSessionList([FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetWorkSessionsListQuery { Start = start, End = end };
            var handler = _workSessionService.GetWorkSessionsList();
            var sessions = await handler.Handle(query);
            return Ok(sessions);
        }
    }
}
