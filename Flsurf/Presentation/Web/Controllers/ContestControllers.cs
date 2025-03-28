using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Contest;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/contest")]
    public class ContestController : ControllerBase
    {
        private readonly IContestService _contestService;

        public ContestController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ContestEntity>> CreateContest([FromBody] CreateContestCommand command)
        {
            var handler = _contestService.CreateContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("approve")]
        public async Task<ActionResult<ContestEntity>> ApproveContest([FromBody] ApproveContestCommand command)
        {
            var handler = _contestService.ApproveContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("start")]
        public async Task<ActionResult<ContestEntity>> StartContest([FromBody] StartContestCommand command)
        {
            var handler = _contestService.StartContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("end")]
        public async Task<ActionResult<ContestEntity>> EndContest([FromBody] EndContestCommand command)
        {
            var handler = _contestService.EndContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<Guid>> DeleteContest([FromBody] DeleteContestCommand command)
        {
            var handler = _contestService.DeleteContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("select-winner")]
        public async Task<ActionResult<CommandResult>> SelectContestWinner([FromBody] SelectContestWinnerCommand command)
        {
            var handler = _contestService.SelectContestWinner();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContestEntity>> GetContest(Guid id)
        {
            var query = new GetContestQuery { ContestId = id };
            var handler = _contestService.GetContest();
            var contest = await handler.Handle(query);
            return contest == null ? NotFound("Конкурс не найден") : Ok(contest);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ContestEntity>>> GetContestList([FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetContestListQuery { Start = start, End = end };
            var handler = _contestService.GetContestList();
            var contests = await handler.Handle(query);
            return Ok(contests);
        }
    }
}
