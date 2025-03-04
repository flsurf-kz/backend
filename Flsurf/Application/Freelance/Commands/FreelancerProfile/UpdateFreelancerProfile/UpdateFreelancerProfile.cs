using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdateFreelancerProfileCommand : BaseCommand
    {
        public List<Guid>? Skills { get; set; } 
        public string? Experience { get; set; }
        public string? Resume { get; set; }
        public decimal? HourlyRate { get; set; }
        public AvailabilityStatus? Availability { get; set; }
    }


    public class UpdateFreelancerProfileHandler : ICommandHandler<UpdateFreelancerProfileCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public UpdateFreelancerProfileHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(UpdateFreelancerProfileCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            var profile = await _dbContext.FreelancerProfiles
                .Include(x => x.User)
                .Include(x => x.PortfolioProjects)
                .FirstOrDefaultAsync(x => x.UserId == currentUser.Id);

            if (profile == null)
            {
                return CommandResult.NotFound("Freelancer profile not found.", currentUser.Id);
            }

            // Обновляем только переданные значения
            if (command.Skills != null && command.Skills.Any())
            {
                var existingSkills = await _dbContext.Skills
                    .Where(s => command.Skills.Contains(s.Id))
                    .ToListAsync();

                profile.Skills = existingSkills.ToList(); // Сохраняем список GUID
            }
            if (command.Experience != null) profile.Experience = command.Experience;
            if (command.Resume != null) profile.Resume = command.Resume;
            if (command.HourlyRate.HasValue) profile.CostPerHour = command.HourlyRate ?? profile.CostPerHour;
            if (command.Availability != null) profile.Availability = command.Availability ?? profile.Availability;

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(currentUser.Id);
        }
    }

}
