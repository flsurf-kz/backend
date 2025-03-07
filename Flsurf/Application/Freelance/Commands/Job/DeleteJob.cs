using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class DeleteJobCommand : BaseCommand
    {
        public Guid JobId { get; set; }
    }

    public class DeleteJobHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<DeleteJobCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(DeleteJobCommand command)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Contract) // Загружаем контракт, если он есть
                .FirstOrDefaultAsync(j => j.Id == command.JobId);

            if (job == null)
            {
                return CommandResult.NotFound("Job not found", command.JobId);
            }

            var user = await _permissionService.GetCurrentUser();

            // Проверяем, является ли пользователь владельцем работы
            bool hasPermission = await _permissionService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanDeleteJob(ZedJob.WithId(job.Id))
            );

            if (!hasPermission)
            {
                return CommandResult.Forbidden("You do not have permission to delete this job.");
            }

            // Если у работы есть активный контракт, нельзя удалять
            if (job.Contract != null)
            {
                return CommandResult.Conflict("Cannot delete job with an active contract.");
            }

            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(job.Id);
        }
    }
}
