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
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chat;

        public ChatController(IChatService chatService)
            => _chat = chatService;

        [HttpGet("list", Name = "GetChats")]
        public async Task<ActionResult<ICollection<ChatEntity>>> GetChats()
        {
            var handler = _chat.GetChatsList();
            var chats = await handler.Execute(new GetChatsListDto());
            return Ok(chats);
        }

        [HttpPost("create", Name = "CreateChat")]
        public async Task<ActionResult<CommandResult>> CreateChat([FromBody] CreateChatDto cmd)
        {
            var handler = _chat.Create();
            var result = await handler.Execute(cmd);
            return CommandResult.Success(result).MapResult(this);
        }

        [HttpPost("close", Name = "CloseChat")]
        public async Task<ActionResult<CommandResult>> CloseChat([FromBody] CloseChatDto cmd)
        {
            var handler = _chat.Close();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("bookmark", Name = "BookmarkChat")]
        public async Task<ActionResult<CommandResult>> BookmarkChat([FromBody] BookmarkChatDto cmd)
        {
            var handler = _chat.Bookmark();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("update", Name = "UpdateChat")]
        public async Task<ActionResult<CommandResult>> UpdateChat([FromBody] UpdateChatDto cmd)
        {
            var handler = _chat.Update();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("mark-as-read", Name = "MarkAsRead")]
        public async Task<ActionResult<CommandResult>> MarkAsRead([FromBody] MarkAsReadDto cmd)
        {
            var handler = _chat.MarkAsRead();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpPost("invite", Name = "InviteMember")]
        public async Task<ActionResult<CommandResult>> InviteMember([FromBody] InviteMemberDto cmd)
        {
            var handler = _chat.Invite();
            var result = await handler.Execute(cmd);
            return Ok(result); 
        }

        [HttpPost("kick", Name = "KickMember")]
        public async Task<ActionResult<CommandResult>> KickMember([FromBody] KickMemberDto cmd)
        {
            var handler = _chat.Kick();
            var result = await handler.Execute(cmd);
            return CommandResult.Success().MapResult(this);
        }

        [HttpGet("{chatId:guid}", Name = "GetChat")]
        public async Task<ActionResult<ChatEntity?>> KickMember(Guid chatId)
        {
            var handler = _chat.GetChat();
            var result = await handler.Execute(chatId);
            return Ok(result); 
        }

        [HttpGet("{chatId:guid}/unread", Name = "GetChatUnreadCounter")]
        public async Task<ActionResult<int>> GetChatUnreadCounter(Guid chatId)
        {
            var handler = _chat.GetUnreadCounter();
            var result = await handler.Execute(new GetUnreadCounterDto() { ChatId = chatId });
            return Ok(result); 
        }

        [HttpGet("unread", Name = "GetUnreadCounter")]
        public async Task<ActionResult<int>> GetUnreadCounter()
        {
            var handler = _chat.GetUnreadCounter();
            var result = await handler.Execute(new GetUnreadCounterDto() { });
            return Ok(result);
        }
    }
}
