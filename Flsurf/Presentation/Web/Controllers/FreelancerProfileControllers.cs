using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.FreelancerProfile;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/freelancer-profile")]
    public class FreelancerProfileController : ControllerBase
    {
        private readonly IFreelancerProfileService _freelancerProfileService;
        public FreelancerProfileController(IFreelancerProfileService freelancerProfileService)
        {
            _freelancerProfileService = freelancerProfileService;
        }

        [HttpPost("create", Name = "CreateFreelancerProfile")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateFreelancerProfile([FromBody] CreateFreelancerProfileCommand command)
        {
            var handler = _freelancerProfileService.CreateFreelancerProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdateFreelancerProfile")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateFreelancerProfile([FromBody] UpdateFreelancerProfileCommand command)
        {
            var handler = _freelancerProfileService.UpdateFreelancerProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("hide", Name = "HideFreelancerProfile")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> HideFreelancerProfile([FromBody] HideFreelancerProfileCommand command)
        {
            var handler = _freelancerProfileService.HideFreelancerProfile();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{userId}", Name = "GetFreelancerProfile")]
        public async Task<ActionResult<FreelancerProfileEntity>> GetFreelancerProfile(Guid userId)
        {
            var query = new GetFreelancerProfileQuery { UserId = userId };
            var handler = _freelancerProfileService.GetFreelancerProfile();
            var profile = await handler.Handle(query);
            if (profile == null)
                return NotFound("Профиль не найден");
            return Ok(profile);
        }

        [HttpGet("list", Name = "GetFreelancerProfileList")]
        public async Task<ActionResult<ICollection<FreelancerProfileEntity>>> GetFreelancerProfileList(
            [FromQuery] int start = 0,
            [FromQuery] int end = 10,
            [FromQuery] string[]? skills = null,
            [FromQuery] int? minCost = null,
            [FromQuery] int? maxCost = null,
            [FromQuery] int? minReviews = null,
            [FromQuery] int? maxReviews = null)
        {
            var query = new GetFreelancerProfileListQuery
            {
                Start = start,
                Ends = end,
                Skills = skills,
                CostPerHour = (minCost.HasValue && maxCost.HasValue) ? new int[] { minCost.Value, maxCost.Value } : null,
                ReviewsCount = (minReviews.HasValue && maxReviews.HasValue) ? new int[] { minReviews.Value, maxReviews.Value } : null
            };

            var handler = _freelancerProfileService.GetFreelancerProfileList();
            var profiles = await handler.Handle(query);
            return Ok(profiles);
        }
    }
}
