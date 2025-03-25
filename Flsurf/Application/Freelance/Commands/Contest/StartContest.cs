using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class StartContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
    }

    public class StartContestHandler : ICommandHandler<StartContestCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly TransactionInnerService _transactionService;

        public StartContestHandler(IApplicationDbContext context, TransactionInnerService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<CommandResult> Handle(StartContestCommand command)
        {
            var contest = await _context.Contests.FirstOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);
            if (contest.Status != ContestStatus.Approved)
                return CommandResult.BadRequest("Конкурс должен быть утвержден перед запуском.");

            var clientWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == contest.EmployerId);
            if (clientWallet == null)
                return CommandResult.NotFound("Кошелёк клиента не найден.", contest.EmployerId);

            var freezeResult = await _transactionService.FreezeAmount(contest.PrizePool, clientWallet.Id, 30);
            if (!freezeResult.IsSuccess)
                return CommandResult.BadRequest("Не удалось заморозить средства на кошельке клиента.");

            contest.Status = ContestStatus.Open;
            await _context.SaveChangesAsync();
            return CommandResult.Success(contest.Id);
        }
    }
}
