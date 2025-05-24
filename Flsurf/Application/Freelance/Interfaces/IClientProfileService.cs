using Flsurf.Application.Freelance.Commands.ClientProfile;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IClientProfileService
    {
        GetClientOrderInfoHandler GetClientOrderInfo();
        GetClientProfileHandler GetClientProfile();
        CreateClientProfileHandler CreateClientProfile();
        SuspendClientProfileHandler SuspendClientProfile();
        UpdateClientProfileHandler UpdateClientProfile();
    }
}
