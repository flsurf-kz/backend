using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Messaging.Services;

namespace Flsurf.Application.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMessageService, MessageService>(); 

            return services; 
        }
    }
}
