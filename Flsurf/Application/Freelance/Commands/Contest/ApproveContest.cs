using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class ApproveContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
    }

    public class ApproveContestHandler : ICommandHandler<ApproveContestCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public ApproveContestHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(ApproveContestCommand command)
        {
            // Получаем текущего пользователя через IPermissionService
            var currentUser = await _permService.GetCurrentUser();

            // Проверяем права: разрешено только модераторам, администраторам
            // либо тем, кто имеет явное разрешение на утверждение конкурса.
            // Здесь предполагается, что для конкурса сформирован объект-права через ZedContest.
            bool hasPermission = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(currentUser.Id).CanApproveContests() 
            );

            if (!hasPermission)
            {
                return CommandResult.Forbidden("idi na"); 
            }

            // Ищем конкурс
            var contest = await _context.Contests.FirstOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);

            // Допустимо утверждать только конкурсы в статусе Moderation
            if (contest.Status != ContestStatus.Moderation)
                return CommandResult.BadRequest("Только конкурсы в статусе 'Moderation' могут быть утверждены.");

            contest.Status = ContestStatus.Approved;
            await _context.SaveChangesAsync();

            return CommandResult.Success(contest.Id);
        }
    }
}
