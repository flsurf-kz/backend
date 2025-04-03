using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Task;
using Flsurf.Application.Freelance.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CommandResult>> CreateTask([FromBody] CreateTaskCommand command)
        {
            var handler = _taskService.Create();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<CommandResult>> CompleteTask([FromBody] CompleteTaskCommand command)
        {
            var handler = _taskService.Complete();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("react")]
        public async Task<ActionResult<CommandResult>> ReactToTask([FromBody] ReactToTaskCommand command)
        {
            var handler = _taskService.React();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        public async Task<ActionResult<CommandResult>> UpdateTask([FromBody] UpdateTaskCommand command)
        {
            var handler = _taskService.Update();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<CommandResult>> DeleteTask([FromBody] DeleteTaskCommand command)
        {
            var handler = _taskService.Delete();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }
    }
}
