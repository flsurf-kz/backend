using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class DeleteContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
    }

    public class DeleteContestHandler : ICommandHandler<DeleteContestCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteContestHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResult> Handle(DeleteContestCommand command)
        {
            var contest = await _context.Contests.FirstOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);

            if (contest.Status != ContestStatus.Draft && contest.Status != ContestStatus.Moderation)
                return CommandResult.BadRequest("Конкурс можно удалить только в статусе 'Draft' или 'Moderation'.");

            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();

            return CommandResult.Success(contest.Id);
        }
    }
}
