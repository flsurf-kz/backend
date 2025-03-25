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
                .FirstOrDefaultAsync(w => w.UserId == contest.EmployerId);
            if (clientWallet == null)
                return CommandResult.NotFound("Кошелёк клиента не найден.", contest.EmployerId);
                
            if (contest.ContestEntries.Count <= 2)
            {
                // деньги исчезают 
                var unfreezeResult = await _transactionService.UnfreezeAmount(contest.PrizePool, clientWallet.Id);
                var balanceOpertionResult = await _transactionService.BalanceOperation(contest.PrizePool * .1, clientWallet.Id, Domain.Payment.Enums.BalanceOperationType.Withdrawl); 

                if (!unfreezeResult.IsSuccess || !balanceOpertionResult.IsSuccess)
                    return CommandResult.BadRequest("Ошибка возврата средств с применением штрафа.");
            } else
            {
                if (contest.WinnerEntryId == null)
                    return CommandResult.Conflict("Нельзя закончить без выигрывшего пользвателя");
                    var bestEntry = contest.ContestEntries.Where(x => x.Id == contest.WinnerEntryId).FirstOrDefault();
                if (bestEntry == null)
                    return CommandResult.BadRequest("Нет поданных заявок.");
                var freelancerWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == bestEntry.FreelancerId);
                if (freelancerWallet == null)
                    return CommandResult.NotFound("Кошелёк фрилансера не найден.", bestEntry.FreelancerId);
                var transferResult = await _transactionService.Transfer(contest.PrizePool, freelancerWallet.Id, clientWallet.Id, feePolicy: null, freezeForDays: 14);
                if (!transferResult.IsSuccess)
                    return CommandResult.BadRequest("Ошибка перевода средств победному фрилансеру.");
            }

            await _context.SaveChangesAsync();
            return CommandResult.Success(contest.Id);
        }
    }
}
