using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.Events;
using Microsoft.EntityFrameworkCore;


namespace Flsurf.Application.User.EventHandlers
{

    public class UserCreatedIntegrationHandler(
            ILogger<UserCreatedIntegrationHandler> logger,
            IApplicationDbContext ctx,          // читаем пользователя после коммита
            IFreelancerProfileService freelancer,
            IClientProfileService client)
        : IIntegrationEventSubscriber<UserCreated>
    {
        private const string DEFAULT_PHONE = "7777777777";

        public async Task HandleEvent(UserCreated evt)
        {
            var user = await ctx.Users
                                .AsNoTracking()
                                .FirstAsync(x => x.Id == evt.UserId);

            CommandResult result; 

            switch (user.Type)
            {
                case UserTypes.Freelancer:
                    result = await freelancer.CreateFreelancerProfile()
                                    .Handle(new Freelance.Commands.FreelancerProfile
                                                  .CreateFreelancerProfileCommand
                                    {
                                        UserId = user.Id,
                                        Resume = string.Empty,
                                        Experience = string.Empty,
                                        HourlyRate = 0
                                    });
                    if (!result.IsSuccess)
                    {
                        // it means freelancer profile was created 
                        if (result.Status != System.Net.HttpStatusCode.Conflict && result.Status != System.Net.HttpStatusCode.Forbidden)
                        {
                            logger.LogError(result.Message + "Status" + result.Status.ToString());
                            throw new Exception(result.Message);
                        }
                    }
                    logger.LogInformation("INTEGRATION‑handler → freelancer profile created for {Id}", user.Id);
                    break;

                case UserTypes.Client:
                    result = await client.CreateClientProfile().Handle(
                               new Freelance.Commands.ClientProfile
                                     .CreateClientProfileCommand
                               {
                                   UserId = user.Id,
                                   CompanyName = string.Empty,
                                   EmployerType = ClientType.Indivdual,
                                   PhoneNumber = user.Phone ?? DEFAULT_PHONE
                               });

                    if (!result.IsSuccess)
                    {
                        if (result.Status != System.Net.HttpStatusCode.Conflict && result.Status != System.Net.HttpStatusCode.Forbidden)
                        {
                            logger.LogError(result.Message + ",[Critical]Status:" + result.Status.ToString());
                            throw new Exception(result.Message);
                        }
                    }
                    logger.LogInformation("INTEGRATION‑handler → client profile created for {Id}", user.Id);
                    break;

                default:
                    // staff / external — ничего не делаем
                    break;
            }
        }
    }

}
