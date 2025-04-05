using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.FreelancerTeam;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/freelancer-team")]
    public class FreelancerTeamController : ControllerBase
    {
        private readonly IFreelancerTeamService _freelancerTeamService;
        public FreelancerTeamController(IFreelancerTeamService freelancerTeamService)
        {
            _freelancerTeamService = freelancerTeamService;
        }

        [HttpPost("create", Name = "CreateFreelancerTeam")]
        public async Task<ActionResult<CommandResult>> CreateFreelancerTeam([FromBody] CreateFreelancerTeamCommand command)
        {
            var handler = _freelancerTeamService.CreateFreelancerTeam();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdateFreelancerTeam")]
        public async Task<ActionResult<CommandResult>> UpdateFreelancerTeam([FromBody] UpdateFreelancerTeamCommand command)
        {
            var handler = _freelancerTeamService.UpdateFreelancerTeam();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete", Name = "DeleteFreelancerTeam")]
        public async Task<ActionResult<CommandResult>> DeleteFreelancerTeam([FromBody] DeleteFreelancerTeamCommand command)
        {
            var handler = _freelancerTeamService.DeleteFreelancerTeam();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list", Name = "GetFreelancerTeams")]
        public async Task<ActionResult<ICollection<FreelancerTeamEntity>>> GetFreelancerTeams()
        {
            var handler = _freelancerTeamService.GetFreelancerTeams();
            var teams = await handler.Handle(new GetFreelancerTeamsListQuery());
            return Ok(teams);
        }
    }
}
