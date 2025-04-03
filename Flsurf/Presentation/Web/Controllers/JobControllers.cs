﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Job;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Hangfire.Common;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CommandResult>> CreateJob([FromBody] CreateJobCommand command)
        {
            var handler = _jobService.CreateJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        public async Task<ActionResult<CommandResult>> UpdateJob([FromBody] UpdateJobCommand command)
        {
            var handler = _jobService.UpdateJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<CommandResult>> DeleteJob([FromBody] DeleteJobCommand command)
        {
            var handler = _jobService.DeleteJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(Guid id)
        {
            var query = new GetJobQuery { JobId = id };
            var handler = _jobService.GetJob();
            var job = await handler.Handle(query);
            if (job == null)
                return NotFound("Вакансия не найдена");
            return Ok(job);
        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<Job>>> GetJobsList(
            [FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetJobsListQuery { Start = start, Ends = end };
            var handler = _jobService.GetJobsList();
            var jobs = await handler.Handle(query);
            return Ok(jobs);
        }

        [HttpPost("bookmark")]
        public async Task<ActionResult<CommandResult>> BookmarkJob([FromBody] BookmarkJobCommand command)
        {
            var handler = _jobService.BookmarkJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("hide")]
        public async Task<ActionResult<CommandResult>> HideJob([FromBody] HideJobCommand command)
        {
            var handler = _jobService.HideJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("submit-proposal")]
        public async Task<ActionResult<CommandResult>> SubmitProposal([FromBody] SubmitProposalCommand command)
        {
            var handler = _jobService.SubmitProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update-proposal")]
        public async Task<ActionResult<CommandResult>> UpdateProposal([FromBody] UpdateProposalCommand command)
        {
            var handler = _jobService.UpdateProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("withdraw-proposal")]
        public async Task<ActionResult<CommandResult>> WithdrawProposal([FromBody] WithdrawProposalCommand command)
        {
            var handler = _jobService.WithdrawProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("bookmarks")]
        public async Task<ActionResult<ICollection<JobEntity>>> GetBookmarksList()
        {
            var handler = _jobService.GetBookmarksList();
            var bookmarks = await handler.Handle(new GetBookmarksListQuery());
            return Ok(bookmarks);
        }
    }
}
