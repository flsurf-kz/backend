﻿using Flsurf.Application.Common.cqrs;
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

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreatePortfolioProject([FromBody] AddPortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.CreatePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdatePortfolioProject([FromBody] UpdatePortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.UpdatePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> DeletePortfolioProject([FromBody] DeletePortfolioProjectCommand command)
        {
            var handler = _portfolioProjectService.DeletePortfolioProject();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<ICollection<PortfolioProjectEntity>>> GetPortfolioProjects()
        {
            var handler = _portfolioProjectService.GetPortfolioProjects();
            var projects = await handler.Handle(new GetPortfolioProjectsQuery());
            return Ok(projects);
        }
    }
}
