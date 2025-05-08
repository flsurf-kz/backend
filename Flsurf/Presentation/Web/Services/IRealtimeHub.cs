using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Flsurf.Presentation.Web.Services
{
    public interface IRealtimeHub
    {
        Task PushAsync(Guid userId, string json);
        bool AddWs(Guid userId, WebSocket ws);
        void RemoveWs(Guid userId);
        bool AddSse(Guid userId, HttpResponse res);
        void RemoveSse(Guid userId);
    }

    public class InMemoryRealtimeHub : IRealtimeHub
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _ws = new();
        private readonly ConcurrentDictionary<Guid, HttpResponse> _sse = new();

        public bool AddWs(Guid id, WebSocket ws) => _ws.TryAdd(id, ws);
        public void RemoveWs(Guid id) => _ws.TryRemove(id, out _);
        public bool AddSse(Guid id, HttpResponse r) => _sse.TryAdd(id, r);
        public void RemoveSse(Guid id) => _sse.TryRemove(id, out _);

        public async Task PushAsync(Guid id, string json)
        {
            if (_ws.TryGetValue(id, out var ws) && ws.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                return;
            }
            if (_sse.TryGetValue(id, out var res) && !res.HttpContext.RequestAborted.IsCancellationRequested)
            {
                await res.WriteAsync($"data:{json}\n\n");
                await res.Body.FlushAsync();
            }
        }
    }

}
