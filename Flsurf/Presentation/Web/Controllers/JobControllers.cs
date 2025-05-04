using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Job;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Application.Freelance.Queries.Responses;
using Flsurf.Domain.Freelance.Entities;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/job")]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("create", Name = "CreateJob")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateJob([FromBody] CreateJobCommand command)
        {
            var handler = _jobService.CreateJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("react-sent-job", Name = "ReactToSentJob")]
        public async Task<ActionResult<CommandResult>> ReactToSentJob([FromBody] ReactToSentJobCommand command)
        {
            var handler = _jobService.ReactToSentJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("sent-draft-to-mod", Name = "SentDraftToMod")]
        public async Task<ActionResult<CommandResult>> SentDraftToMod([FromBody] SendDraftJobToModerationCommand command)
        {
            var handler = _jobService.SendDraftJobToModeration();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdateJob")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateJob([FromBody] UpdateJobCommand command)
        {
            var handler = _jobService.UpdateJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete", Name = "DeleteJob")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> DeleteJob([FromBody] DeleteJobCommand command)
        {
            var handler = _jobService.DeleteJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}", Name = "GetJob")]
        public async Task<ActionResult<JobDetails>> GetJob(Guid id)
        {
            var query = new GetJobQuery { JobId = id };
            var handler = _jobService.GetJob();
            var job = await handler.Handle(query);
            if (job == null)
                return NotFound("Вакансия не найдена");
            return Ok(job);
        }

        [HttpGet("{id}/raw", Name = "GetRawJob")]
        public async Task<ActionResult<JobEntity>> GetJobRaw(Guid id)
        {
            var query = new GetRawJobQuery { JobId = id };
            var handler = _jobService.GetRawJob();
            var job = await handler.Handle(query);
            return Ok(job);
        }

        [HttpPost("list", Name = "GetJobsList")]
        public async Task<ActionResult<ICollection<JobEntity>>> GetJobsList(
            [FromBody] GetJobsListQuery query)
        {
            var handler = _jobService.GetJobsList();
            var jobs = await handler.Handle(query);
            return Ok(jobs);
        }

        [HttpPost("bookmark", Name = "BookmarkJob")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> BookmarkJob([FromBody] BookmarkJobCommand command)
        {
            var handler = _jobService.BookmarkJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("hide", Name = "HideJob")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> HideJob([FromBody] HideJobCommand command)
        {
            var handler = _jobService.HideJob();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("submit-proposal", Name = "SubmitProposal")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> SubmitProposal([FromBody] SubmitProposalCommand command)
        {
            var handler = _jobService.SubmitProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update-proposal", Name = "UpdateProposal")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateProposal([FromBody] UpdateProposalCommand command)
        {
            var handler = _jobService.UpdateProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("withdraw-proposal", Name = "WithdrawProposal")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> WithdrawProposal([FromBody] WithdrawProposalCommand command)
        {
            var handler = _jobService.WithdrawProposal();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("bookmarks", Name = "GetBookmarksList")]
        [Authorize]
        public async Task<ActionResult<ICollection<JobEntity>>> GetBookmarksList()
        {
            var handler = _jobService.GetBookmarksList();
            var bookmarks = await handler.Handle(new GetBookmarksListQuery());
            return Ok(bookmarks);
        }
    }
}
