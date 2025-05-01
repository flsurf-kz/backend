using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class SendDraftJobToModerationCommand : BaseCommand
    {
        public Guid JobId { get; set; }
    }

    public class SendDraftJobToModerationHandler : ICommandHandler<SendDraftJobToModerationCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _perm;

        public SendDraftJobToModerationHandler(IApplicationDbContext db, IPermissionService perm)
        {
            _db = db;
            _perm = perm;
        }

        public async Task<CommandResult> Handle(SendDraftJobToModerationCommand cmd)
        {
            var user = await _perm.GetCurrentUser();
            var job = await _db.Jobs
                .Include(j => j.Contract)
                .FirstOrDefaultAsync(j => j.Id == cmd.JobId);

            if (job == null)
                return CommandResult.NotFound("Вакансия не найдена", cmd.JobId);

            // Только заказчик контракта может отправлять
            if (job.EmployerId != user.Id)
                return CommandResult.Forbidden("Нет прав отправить на модерацию");

            if (job.Status != JobStatus.Draft)
                return CommandResult.BadRequest("Только вакансии в состоянии Draft можно отправить на модерацию");

            job.Status = JobStatus.SentToModeration;
            await _db.SaveChangesAsync();

            return CommandResult.Success(job.Id);
        }
    }
}
