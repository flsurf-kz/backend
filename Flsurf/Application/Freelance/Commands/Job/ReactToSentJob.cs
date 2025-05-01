using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class ReactToSentJobCommand : BaseCommand
    {
        public Guid JobId { get; set; }
        public JobReaction Reaction { get; set; }
    }

    public class ReactToSentJobHandler : ICommandHandler<ReactToSentJobCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _perm;

        public ReactToSentJobHandler(IApplicationDbContext db, IPermissionService perm)
        {
            _db = db;
            _perm = perm;
        }

        public async Task<CommandResult> Handle(ReactToSentJobCommand cmd)
        {
            var user = await _perm.GetCurrentUser();
            // проверяем право модерации
            var zedUser = ZedFreelancerUser.WithId(user.Id);
            var allowed = await _perm.CheckPermission(zedUser.CanApproveJobs()); 
            if (!allowed)
                return CommandResult.Forbidden("Нет прав модератора вакансий");

            var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == cmd.JobId);
            if (job == null)
                return CommandResult.NotFound("Вакансия не найдена", cmd.JobId);

            if (job.Status != JobStatus.SentToModeration)
                return CommandResult.BadRequest("Вакансия не находится на модерации");

            switch (cmd.Reaction)
            {
                case JobReaction.Resubmit:
                    job.Status = JobStatus.Draft;
                    break;

                case JobReaction.Approve:
                    job.Status = JobStatus.Open;
                    break;

                case JobReaction.Delete:
                    // TODO! 
                    break;
            }

            await _db.SaveChangesAsync();
            return CommandResult.Success(job.Id);
        }
    }
}
