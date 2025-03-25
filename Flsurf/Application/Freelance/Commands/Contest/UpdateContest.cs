using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class UpdateContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? PrizePool { get; set; }
    }

    public class UpdateContestHandler : ICommandHandler<UpdateContestCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public UpdateContestHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(UpdateContestCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();
            var contest = await _context.Contests.FirstOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);
            if (contest.EmployerId != currentUser.Id)
                return CommandResult.Forbidden("Нет прав на изменение конкурса.");

            if ((contest.Status == ContestStatus.Approved || contest.Status == ContestStatus.Open) &&
                (!string.IsNullOrWhiteSpace(command.Title) || !string.IsNullOrWhiteSpace(command.Description)))
            {
                return CommandResult.BadRequest("После публикации нельзя изменять заголовок и описание конкурса.");
            }

            if (!string.IsNullOrWhiteSpace(command.Title))
                contest.Title = command.Title;
            if (!string.IsNullOrWhiteSpace(command.Description))
                contest.Description = command.Description;
            if (command.PrizePool.HasValue)
                contest.PrizePool = new Money(command.PrizePool.Value);

            await _context.SaveChangesAsync();
            return CommandResult.Success(contest.Id);
        }
    }
}
