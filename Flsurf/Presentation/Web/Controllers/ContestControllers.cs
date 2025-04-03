using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Contest;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public async Task<ActionResult<CommandResult>> CreateContest([FromBody] CreateContestCommand command)
        {
            var handler = _contestService.CreateContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("approve")]
        public async Task<ActionResult<CommandResult>> ApproveContest([FromBody] ApproveContestCommand command)
        {
            var handler = _contestService.ApproveContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("start")]
        public async Task<ActionResult<CommandResult>> StartContest([FromBody] StartContestCommand command)
        {
            var handler = _contestService.StartContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("end")]
        public async Task<ActionResult<CommandResult>> EndContest([FromBody] EndContestCommand command)
        {
            var handler = _contestService.EndContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<CommandResult>> DeleteContest([FromBody] DeleteContestCommand command)
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

        [HttpPost("submit-entry")]
        public async Task<ActionResult<CommandResult>> SubmitContestEntry([FromBody] SubmitContestEntryCommand command)
        {
            var handler = _contestService.SubmitContestEntry();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete-entry")]
        public async Task<ActionResult<CommandResult>> DeleteContestEntry([FromBody] DeleteContestEntryCommand command)
        {
            var handler = _contestService.DeleteContestEntry();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        public async Task<ActionResult<CommandResult>> UpdateContest([FromBody] UpdateContestCommand command)
        {
            var handler = _contestService.UpdateContest();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContestEntity>> GetContest(Guid id)
        {
            var query = new GetContestQuery { ContestId = id };
            var handler = _contestService.GetContest();
            var contest = await handler.Handle(query);
            if (contest == null)
                return NotFound("Конкурс не найден");
            return Ok(contest);
        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<ContestEntity>>> GetContestList(
            [FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetContestListQuery { Start = start, Ends = end };
            var handler = _contestService.GetContestList();
            var contests = await handler.Handle(query);
            return Ok(contests);
        }
    }
}
