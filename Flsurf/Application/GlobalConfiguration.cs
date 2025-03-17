using Flsurf.Infrastructure.EventDispatcher;
using System.Reflection;

namespace Flsurf.Application
{
    public static class GlobalConfiguration
    {
        public static IApplicationBuilder UseEventDispatcher(this IApplicationBuilder app)
        {
            var eventDispatcher = (EventDispatcher)app.ApplicationServices.GetRequiredService<IEventDispatcher>();

            eventDispatcher.AddIgnoredTypes([typeof(LoggingHandler<>)]);

            // Resolve and add event listeners within the scope of a request
            using (var scope = app.ApplicationServices.CreateScope())
            {
                eventDispatcher.RegisterEventSubscribers(Assembly.GetExecutingAssembly(), scope);
            }

            return app;
        }
    }
}
