using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Skills;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Application.Freelance.Queries.Responses;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/skill")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpPost("create", Name = "CreateSkills")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateSkills([FromBody] CreateSkillsCommand command)
        {
            var handler = _skillService.CreateSkills();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdateSkills")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateSkills([FromBody] UpdateSkillsCommand command)
        {
            var handler = _skillService.UpdateSkills();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete", Name = "DeleteSkills")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> DeleteSkills([FromBody] DeleteSkillsCommand command)
        {
            var handler = _skillService.DeleteSkills();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list", Name = "GetSkills")]
        public async Task<ActionResult<ICollection<SkillModel>>> GetSkills([FromQuery] string? searchQuery)
        {
            var handler = _skillService.GetSkills();
            var skills = await handler.Handle(new GetSkillsQuery() { SearchQuery = searchQuery});
            return Ok(skills);
        }

        [HttpGet("{skillId}", Name = "GetSkill")]
        public async Task<ActionResult<SkillModel?>> GetSkill(Guid skillId)
        {
            var result = await _skillService.GetSkill().Handle(new GetSkillQuery() { SkillId = skillId });

            return result; 
        } 
    }
}
