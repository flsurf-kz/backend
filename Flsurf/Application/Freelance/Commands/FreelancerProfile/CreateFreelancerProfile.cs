using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerProfile
{
    public class CreateFreelancerProfileCommand : BaseCommand
    {
        // for internal usage 
        public Guid? UserId { get; set; } 
        public string Experience { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public string? Resume { get; set; }
    }


    public class CreateFreelancerProfileHandler : ICommandHandler<CreateFreelancerProfileCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public CreateFreelancerProfileHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateFreelancerProfileCommand command)
        {
            UserEntity? user;

            if (command.UserId == null)
                user = await _permService.GetCurrentUser();
            else
                user = await _dbContext.Users
                    .IncludeStandard()
                    .FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user == null)
                return CommandResult.NotFound("ПОшел нахуй отсюда", command.UserId ?? Guid.Empty);

            if (user.Type != Domain.User.Enums.UserTypes.Freelancer)
            {
                return CommandResult.Conflict("You are not freelancer"); 
            }

            // Проверяем, что профиль еще не создан
            var existingProfile = await _dbContext.FreelancerProfiles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingProfile != null)
            {
                return CommandResult.Conflict("Freelancer profile already exists.");
            }

            // Создаем новый профиль
            var profile = FreelancerProfileEntity.Create(user.Id, command.Experience, command.HourlyRate, command.Resume);
            user.ChangeUserType(Domain.User.Enums.UserTypes.Freelancer); 

            _dbContext.FreelancerProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(profile.Id);
        }
    }

}
