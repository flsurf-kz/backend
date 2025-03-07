using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdateJobCommand : BaseCommand
    {
        public Guid JobId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public ICollection<Guid>? RequiredSkillIds { get; set; } = [];
        public Guid? CategoryId { get; set; }
        public decimal? Budget { get; set; }
        public decimal? HourlyRate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Duration { get; set; }
    }

    // TODO: FIX THIS MESS!!
    public class UpdateJobHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<UpdateJobCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(UpdateJobCommand command)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.RequiredSkills)
                .FirstOrDefaultAsync(j => j.Id == command.JobId);

            if (job == null)
            {
                return CommandResult.NotFound("Job not found", command.JobId);
            }

            var user = await _permissionService.GetCurrentUser();

            // Проверяем, является ли пользователь владельцем работы
            bool isOwner = await _permissionService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanDeleteJob(ZedJob.WithId(job.Id))
            );

            if (!isOwner)
            {
                return CommandResult.Forbidden("You can only update jobs that you own.");
            }

            // Нельзя редактировать работу, если она уже закрыта
            if (job.Status == JobStatus.Closed)
            {
                return CommandResult.Conflict("Cannot update a closed job.");
            }

            job.Title = command.Title ?? job.Title;
            job.Description = command.Description ?? job.Description;
            job.Budget = command.Budget ?? job.Budget;
            job.HourlyRate = command.HourlyRate ?? job.HourlyRate;
            job.ExpirationDate = command.ExpirationDate ?? job.ExpirationDate;
            job.Duration = command.Duration ?? job.Duration;

            // Обновляем навыки
            var skills = await _dbContext.Skills.Where(s => command.RequiredSkillIds.Contains(s.Id)).ToListAsync();
            job.RequiredSkills = skills;

            // Обновляем категорию
            var category = await _dbContext.Categories.FindAsync(command.CategoryId);
            if (category == null)
            {
                return CommandResult.NotFound("Category not found", command.CategoryId);
            }
            job.Category = category;

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(job.Id);
        }
    }

}
