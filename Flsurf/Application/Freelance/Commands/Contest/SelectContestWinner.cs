using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class SelectContestWinnerCommand : BaseCommand
    {
        [Required]
        public Guid ContestId { get; set; }
        [Required]
        public Guid EntryId { get; set; }
    }

    public class SelectContestWinnerHandler : ICommandHandler<SelectContestWinnerCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly TransactionInnerService _txService; 

        public SelectContestWinnerHandler(
            IApplicationDbContext context, IPermissionService permService, TransactionInnerService txService)
        {
            _context = context;
            _permService = permService;
            _txService = txService; 
        }

        public async Task<CommandResult> Handle(SelectContestWinnerCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();
            

            var contest = await _context.Contests
                .Include(c => c.ContestEntries)
                .FirstOrDefaultAsync(c => c.Id == command.ContestId);

            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);

            if (contest.EmployerId != currentUser.Id)
                return CommandResult.Forbidden("Пшл нах"); 

            if (contest.ContestEntries.Count == 0)
                return CommandResult.BadRequest("В конкурсе отсутствуют заявки.");

            var bestEntry = await _context.ContestEntries
                .Include(x => x.Freelancer)
                .FirstOrDefaultAsync(x => x.Id == command.EntryId);

            if (bestEntry == null)
                return CommandResult.NotFound("Заявка не найдена", command.EntryId); 

            contest.SelectWinner(bestEntry);

            var freelancerWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == bestEntry.FreelancerId);

            if (freelancerWallet == null)
                return CommandResult.NotFound("", bestEntry.FreelancerId);

            var clientWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == contest.EmployerId);

            if (clientWallet == null)
                return CommandResult.NotFound("", contest.EmployerId);

            await _txService.Transfer(contest.PrizePool, freelancerWallet.Id, clientWallet.Id, new NoFeePolicy(), 14); 

            await _context.SaveChangesAsync();
            return CommandResult.Success(contest.Id);
        }
    }
}
