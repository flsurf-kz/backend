using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class EndContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
    }

    public class EndContestHandler : ICommandHandler<EndContestCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly TransactionInnerService _transactionService;
        private static readonly Guid SYSTEM_WALLET_ID = new Guid("00000000-0000-0000-0000-000000000001");

        public EndContestHandler(IApplicationDbContext context, TransactionInnerService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<CommandResult> Handle(EndContestCommand command)
        {
            var contest = await _context.Contests
                .Include(c => c.ContestEntries)
                .FirstOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);
            if (contest.Status != ContestStatus.Open)
                return CommandResult.BadRequest("Конкурс должен быть открыт для завершения.");
            contest.Status = ContestStatus.Ended;

            var clientWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == contest.CreatedBy);
            if (clientWallet == null)
                return CommandResult.NotFound("Кошелёк клиента не найден.", contest.CreatedBy);

            if (contest.ContestEntries.Count < 2)
            {
                var unfreezeResult = await _transactionService.UnfreezeAmount(contest.PrizePool, clientWallet.Id);
                var penalty = new Money(contest.PrizePool * 0.1m);
                var penaltyResult = await _transactionService.Transfer(penalty, SYSTEM_WALLET_ID, clientWallet.Id, feePolicy: null, freezeForDays: null);
                if (!unfreezeResult.IsSuccess || !penaltyResult.IsSuccess)
                    return CommandResult.BadRequest("Ошибка возврата средств с применением штрафа.");
            }
            else
            {
                var bestEntry = contest.ContestEntries.Where(x => x.Id == contest.WinnerEntryId).FirstOrDefault(); 
                if (bestEntry == null)
                    return CommandResult.BadRequest("Нет поданных заявок.");
                var freelancerWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == bestEntry.FreelancerId);
                if (freelancerWallet == null)
                    return CommandResult.NotFound("Кошелёк фрилансера не найден.", bestEntry.FreelancerId);
                var transferResult = await _transactionService.Transfer(new Money(contest.Reward), freelancerWallet.Id, clientWallet.Id, feePolicy: null, freezeForDays: 14);
                if (!transferResult.IsSuccess)
                    return CommandResult.BadRequest("Ошибка перевода средств победному фрилансеру.");
            }

            await _context.SaveChangesAsync();
            return CommandResult.Success(contest.Id);
        }
    }
}
