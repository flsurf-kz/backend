using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.ClientProfile
{
    public class UpdateClientProfileCommand : BaseCommand
    {
        public Guid ClientId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Location { get; set; }
        public CreateFileDto? CompanyLogo { get; set; }
        public ClientType? EmployerType { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UpdateClientProfileHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService, 
        UploadFile uploadFile)
        : ICommandHandler<UpdateClientProfileCommand>
    {
        public async Task<CommandResult> Handle(UpdateClientProfileCommand command)
        {
            var user = await permService.GetCurrentUser();

            var profile = await dbContext.ClientProfiles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (profile == null)
            {
                return CommandResult.NotFound("Client profile not found.", user.Id);
            }

            // Обновляем данные профиля только если переданы новые значения
            profile.CompanyName = command.CompanyName ?? profile.CompanyName;
            profile.CompanyDescription = command.CompanyDescription ?? profile.CompanyDescription;
            profile.CompanyWebsite = command.CompanyWebsite ?? profile.CompanyWebsite;
            profile.Location = command.Location ?? profile.Location;

            if (command.CompanyLogo != null)
            {
                var file = await uploadFile.Execute(command.CompanyLogo);
                profile.CompanyLogo = file; 
            }

            profile.ClientType = command.EmployerType ?? profile.ClientType;
            profile.PhoneNumber = command.PhoneNumber ?? profile.PhoneNumber;

            await dbContext.SaveChangesAsync();

            return CommandResult.Success(profile.Id);
        }
    }
}
