using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Messaging.UseCases;
using Flsurf.Domain.Messanging.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _msg;

        public MessageController(IMessageService messageService)
            => _msg = messageService;

        [HttpGet("list", Name = "GetMessages")]
        public async Task<ActionResult<ICollection<MessageEntity>>> GetMessages(
            [FromQuery] GetMessagesListDto query)
        {
            var handler = _msg.GetMessages();
            var msgs = await handler.Execute(query);
            return Ok(msgs);
        }

        [HttpPost("send", Name = "SendMessage")]
        public async Task<ActionResult<MessageEntity>> Send([FromBody] SendMessageDto cmd)
        {
            var handler = _msg.Send();
            var result = await handler.Execute(cmd);
            return Ok(result); 
        }

        [HttpPost("update", Name = "UpdateMessage")]
        public async Task<ActionResult<CommandResult>> Update([FromBody] UpdateMessageDto cmd)
        {
            var handler = _msg.Update();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("delete", Name = "DeleteMessage")]
        public async Task<ActionResult<CommandResult>> Delete([FromBody] DeleteMessageDto cmd)
        {
            var handler = _msg.Delete();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("pin", Name = "PinMessage")]
        public async Task<ActionResult<bool>> Pin([FromBody] PinMessageDto cmd)
        {
            var handler = _msg.Pin();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this); 
        }

        [HttpGet("{messageId:guid}/thread", Name = "GetThreadMessages")]
        public async Task<ActionResult<List<MessageThreadDto>>> GetThreadMessages(Guid messageId)
        {
            var handler = _msg.GetThread();
            var result = await handler.Execute(new GetMessagesThreadDto() { MessageId = messageId });
            return CommandResult.Success().MapResult(this);
        }
    }
}
