using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerProfile
{
    public class HideFreelancerProfileCommand : BaseCommand
    {
        public Guid UserId { get; set; } // ID фрилансера, чей профиль скрываем
    }

    public class HideFreelancerProfileHandler : ICommandHandler<HideFreelancerProfileCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public HideFreelancerProfileHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(HideFreelancerProfileCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            // Ищем профиль фрилансера
            var profile = await _dbContext.FreelancerProfiles
                .FirstOrDefaultAsync(x => x.UserId == command.UserId);

            if (profile == null)
            {
                return CommandResult.NotFound("Freelancer profile not found.", command.UserId);
            }

            // Проверяем права: сам фрилансер или модератор
            bool isOwner = currentUser.Id == command.UserId;
            bool isModerator = currentUser.Role == UserRoles.Moderator;

            if (!isOwner && !isModerator)
            {
                return CommandResult.Forbidden("You do not have permission to hide this profile.");
            }

            profile.IsHidden = !profile.IsHidden; 

            // Помечаем профиль как скрытый
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(command.UserId);
        }
    }

}
