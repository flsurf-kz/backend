using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.ClientProfile
{
    public class CreateClientProfileCommand : BaseCommand
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string CompanyWebsite { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string CompanyLogo { get; set; } = string.Empty;
        public string EmployerType { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class CreateClientProfileHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)
        : ICommandHandler<CreateClientProfileCommand>
    {
        public async Task<CommandResult> Handle(CreateClientProfileCommand command)
        {
            var user = await permService.GetCurrentUser();

            if (user.Type != Domain.User.Enums.UserTypes.NonUser)
            {
                return CommandResult.Conflict("You are already registered as a client.");
            }

            // Проверяем, что профиль еще не создан
            var existingProfile = await dbContext.ClientProfiles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingProfile != null)
            {
                return CommandResult.Conflict("Client profile already exists.");
            }

            // Создаем новый профиль
            var profile = new ClientProfileEntity
            {
                UserId = user.Id,
                CompanyName = command.CompanyName,
                CompanyDescription = command.CompanyDescription,
                CompanyWebsite = command.CompanyWebsite,
                Location = command.Location,
                CompanyLogo = command.CompanyLogo,
                EmployerType = command.EmployerType,
                PhoneNumber = command.PhoneNumber
            };

            user.Type = Domain.User.Enums.UserTypes.Client;

            dbContext.ClientProfiles.Add(profile);
            await dbContext.SaveChangesAsync();

            return CommandResult.Success(profile.Id);
        }
    }
}
