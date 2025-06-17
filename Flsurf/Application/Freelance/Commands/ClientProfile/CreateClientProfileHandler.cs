using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Commands.ClientProfile
{
    public class CreateClientProfileCommand : BaseCommand
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyDescription { get; set; } = string.Empty;
        public string? CompanyWebsite { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public CreateFileDto? CompanyLogo { get; set; } 
        public ClientType EmployerType { get; set; }
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class CreateClientProfileHandler(
        IApplicationDbContext dbContext,
        UploadFile uploadFile)
        : ICommandHandler<CreateClientProfileCommand>
    {
        public async Task<CommandResult> Handle(CreateClientProfileCommand command)
        {
            var user = await dbContext.Users
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == command.UserId);

            if (user == null)
                return CommandResult.NotFound("Пользватель не найден", command.UserId); 

            if (user.Type != Domain.User.Enums.UserTypes.NonUser)
            {
                return CommandResult.Forbidden("You are client.");
            }

            // Проверяем, что профиль еще не создан
            var existingProfile = await dbContext.ClientProfiles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingProfile != null)
            {
                return CommandResult.Conflict("Client profile already exists.");
            }

            FileEntity? file = null;
            if (command.CompanyLogo != null)
            {
                file = await uploadFile.Execute(command.CompanyLogo);
            }
            // Создаем новый профиль
            var profile = new ClientProfileEntity
            {
                UserId = user.Id,
                CompanyName = command.CompanyName,
                CompanyDescription = command.CompanyDescription,
                CompanyWebsite = command.CompanyWebsite,
                Location = command.Location,
                CompanyLogo = file,
                ClientType = command.EmployerType,
                PhoneNumber = command.PhoneNumber
            };

            user.ChangeUserType(Domain.User.Enums.UserTypes.Client);

            dbContext.ClientProfiles.Add(profile);
            await dbContext.SaveChangesAsync();

            return CommandResult.Success(profile.Id);
        }
    }
}
