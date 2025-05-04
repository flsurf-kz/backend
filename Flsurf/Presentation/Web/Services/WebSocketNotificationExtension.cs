using System.Net.WebSockets;

namespace Flsurf.Presentation.Web.Services
{
    public static class WebSocketNotificationsExtension
    {
        public static IApplicationBuilder UseWebSocketNotifications(this IApplicationBuilder app)
        {
            app.UseWebSockets();

            return app;
        }
    }

}
