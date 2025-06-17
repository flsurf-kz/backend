using Flsurf.Application.Freelance.Commands.ClientProfile;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class ClientProfileService : IClientProfileService
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientProfileService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public GetClientOrderInfoHandler GetClientOrderInfo() =>
            _serviceProvider.GetRequiredService<GetClientOrderInfoHandler>();

        public GetClientProfileHandler GetClientProfile() =>
            _serviceProvider.GetRequiredService<GetClientProfileHandler>(); 

        public CreateClientProfileHandler CreateClientProfile() =>
            _serviceProvider.GetRequiredService<CreateClientProfileHandler>();

        public SuspendClientProfileHandler SuspendClientProfile() =>
            _serviceProvider.GetRequiredService<SuspendClientProfileHandler>();

        public UpdateClientProfileHandler UpdateClientProfile() =>
            _serviceProvider.GetRequiredService<UpdateClientProfileHandler>();

        public GetClientHistoryHandler GetClientHistory() => 
            _serviceProvider.GetRequiredService<GetClientHistoryHandler>();
    }
}
