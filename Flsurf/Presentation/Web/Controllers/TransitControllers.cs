using System.Net.WebSockets;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Presentation.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly IRealtimeHub _hub;
        private readonly IUser _current;

        public WebSocketController(IRealtimeHub hub, IUser current)
        {
            _hub = hub;
            _current = current;
        }

        [HttpGet]                // GET wss://…/api/ws
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                Response.StatusCode = 400;
                return;
            }

            if (_current.Id is not Guid userId) { Response.StatusCode = 401; return; }

            WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _hub.AddWs(userId, ws);

            var buf = new byte[256];
            try
            {
                while (ws.State == WebSocketState.Open)
                    await ws.ReceiveAsync(buf, CancellationToken.None);   // «глушим»
            }
            finally { _hub.RemoveWs(userId); }
        }
    }

    [ApiController]
    [Authorize]
    [Route("api/sse")]
    public class SseController : ControllerBase
    {
        private readonly IRealtimeHub _hub;
        private readonly IUser _current;

        public SseController(IRealtimeHub hub, IUser current)
        {
            _hub = hub;
            _current = current;
        }

        [HttpGet]                // GET /api/sse
        public async Task Get()
        {
            if (_current.Id is not Guid userId) { Response.StatusCode = 401; return; }

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");

            _hub.AddSse(userId, Response);

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), HttpContext.RequestAborted);
            }
            finally { _hub.RemoveSse(userId); }
        }
    }
}
