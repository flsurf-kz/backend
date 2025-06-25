using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class ClientAcceptFinishContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public int  Rating     { get; set; }
        public string Comment  { get; set; } = string.Empty;
    }


    public class ClientAcceptFinishContractHandler(
        IApplicationDbContext db,
        IPermissionService perm,
        TransactionInnerService tx) 
        : ICommandHandler<ClientAcceptFinishContractCommand>
    {
        public async Task<CommandResult> Handle(ClientAcceptFinishContractCommand cmd)
        {
            var user = await perm.GetCurrentUser();

            var contract = await db.Contracts
                .Include(c => c.Job)
                .FirstOrDefaultAsync(c => c.Id == cmd.ContractId);
            if (contract == null)
                return CommandResult.NotFound("Контракт не найден", cmd.ContractId);

            /* ---------- финансовая часть (как было) ---------- */
            var freelancerWallet = await db.Wallets.FirstAsync(w => w.UserId == contract.FreelancerId);
            if (contract.BudgetType == BudgetType.Fixed)
            {
                await tx.UnfreezeAmount(contract.Budget, freelancerWallet.Id);
                await tx.FreezeAmount  (contract.Budget, freelancerWallet.Id, 14);
            }

            contract.Finish();

            /* ---------- создаём отзыв ---------- */
            var review = JobReviewEntity.Create(
                reviewerId : user.Id,
                targetId   : contract.FreelancerId,
                job        : contract.Job,
                rating     : cmd.Rating,
                comment    : cmd.Comment,
                reviewDate : DateTime.UtcNow);

            db.Reviews.Add(review);

            await db.SaveChangesAsync();
            return CommandResult.Success();
        }
    }
}

