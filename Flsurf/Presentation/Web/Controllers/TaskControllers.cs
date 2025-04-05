using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Task;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Application.Payment.Queries;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateTask([FromBody] CreateTaskCommand command)
        {
            var handler = _taskService.CreateTask();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("complete")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CompleteTask([FromBody] CompleteTaskCommand command)
        {
            var handler = _taskService.CompleteTask();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("react")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ReactToTask([FromBody] ReactToTaskCommand command)
        {
            var handler = _taskService.ReactToTask();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateTask([FromBody] UpdateTaskCommand command)
        {
            var handler = _taskService.UpdateTask();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> DeleteTask([FromBody] DeleteTaskCommand command)
        {
            var handler = _taskService.DeleteTask();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list/{contractId}")]
        [Authorize]
        public async Task<ActionResult<ICollection<TransactionEntity>>> GetTasks(
            Guid contractId, [FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetTasksListQuery() { ContractId = contractId, Start = start, Ends = end };
            var handler = _taskService.GetTasks();
            var transactions = await handler.Handle(query);
            return Ok(transactions);
        }
    }
}
