using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/portfolio-project")]
    public class PortfolioProjectController : ControllerBase
    {
        private readonly IPortfolioProjectService _portfolioProjectService;
        public PortfolioProjectController(IPortfolioProjectService portfolioProjectService)
        {
            _portfolioProjectService = portfolioProjectService;
        }

        [HttpPost("create", Name = "CreatePortfolioProject")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreatePortfolioProject([FromBody] AddPortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.CreatePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdatePortfolioProject")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdatePortfolioProject([FromBody] UpdatePortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.UpdatePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete", Name = "DeletePortfolioProject")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> DeletePortfolioProject([FromBody] DeletePortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.DeletePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list/{userid}", Name = "GetPortfolioProjects")]
        [Authorize]
        public async Task<ActionResult<ICollection<PortfolioProjectEntity>>> GetPortfolioProjects(Guid userId)
        {
            var handler = _portfolioProjectService.GetPortfolioProjects();
            var projects = await handler.Handle(new GetPortfolioProjectsQuery() { FreelancerId = userId });
            return Ok(projects);
        }

        [HttpGet("list/my", Name = "GetMyPortfolioProjects")]
        [Authorize]
        public async Task<ActionResult<ICollection<PortfolioProjectEntity>>> GetMyPortfolioProjects()
        {
            var handler = _portfolioProjectService.GetPortfolioProjects();
            var projects = await handler.Handle(new GetPortfolioProjectsQuery());
            return Ok(projects);
        }
    }
}
