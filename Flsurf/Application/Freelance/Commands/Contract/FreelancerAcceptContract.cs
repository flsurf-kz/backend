using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class FreelancerAcceptContractCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
    }


    public class FreelancerAcceptContractHandler(
            IApplicationDbContext db,
            IPermissionService perm,
            TransactionInnerService txService)
        : ICommandHandler<FreelancerAcceptContractCommand>
    {
        public async Task<CommandResult> Handle(FreelancerAcceptContractCommand cmd)
        {
            /*───────── автор   ───────────────────────────────────────────*/
            var freelancer = await perm.GetCurrentUser();
            if (freelancer.Type != UserTypes.Freelancer)
                return CommandResult.Forbidden("Только фрилансер может принять контракт.");

            /*───────── загрузка сущностей ───────────────────────────────*/
            var contract = await db.Contracts
                .Include(c => c.Employer)
                .Include(c => c.Freelancer)
                .FirstOrDefaultAsync(c => c.Id == cmd.ContractId &&
                                          c.FreelancerId == freelancer.Id);

            if (contract is null)
                return CommandResult.NotFound("Контракт не найден или недоступен.", cmd.ContractId);

            if (contract.Status != ContractStatus.PendingApproval)
                return CommandResult.Conflict("Контракт уже принят или неактуален.");

            var clientWallet = await db.Wallets
                //.Include(x => x.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == contract.EmployerId);
            var freelancerWallet = await db.Wallets
                //.Include(x => x.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == freelancer.Id);

            if (clientWallet is null || freelancerWallet is null)
                return CommandResult.NotFound("Кошельки не найдены.", contract.EmployerId);

            /*───────── расчёт суммы и периода заморозки ─────────────────*/
            Money transferAmount;
            int? freezeDays;

            switch (contract.BudgetType)
            {
                case BudgetType.Fixed:
                    if (contract.Budget == Money.Null())
                        return CommandResult.BadRequest("Не указана сумма фиксированного бюджета.");

                    transferAmount = new Money(contract.Budget); // полная сумма в резерв
                    freezeDays = null;                       // до закрытия контракта
                    break;

                case BudgetType.Hourly:
                    transferAmount = new Money(contract.CostPerHour * 2); // гарантия на 2 часа
                    freezeDays = 14;                                   // разморозка через 14 дней
                    break;

                default:
                    return CommandResult.BadRequest("Неизвестный тип бюджета.");
            }

            /*───────── атомарная транзакция ─────────────────────────────*/
            //await using var trx = await db.Database.BeginTransactionAsync();

            /* 1) Перевод средств в кошельке фрилансера (замороженная часть) */
            //var transfer = await txService.Transfer(
            //    transferAmount,
            //    recieverWallet: freelancerWallet,
            //    senderWallet: clientWallet,
            //    feePolicy: null,      // комиссия 0
            //    freezeForDays: freezeDays);


            var txs = clientWallet.Transfer(transferAmount, clientWallet, null, freezeDays);
            //txs.Item1.RawAmount = new Money(transferAmount);
            //txs.Item2.RawAmount = new Money(transferAmount);
            db.Transactions.AddRange(txs.Item1, txs.Item2);

            //if (!transfer.IsSuccess)
            //{
            //    //await trx.RollbackAsync();
            //    return transfer;
            //}
            /* 2) Активируем контракт и работу */
            contract.Status = ContractStatus.Active;
            contract.StartDate = DateTime.UtcNow;

            var job = await db.Jobs.FirstOrDefaultAsync(j => j.ContractId == contract.Id);
            if (job is not null)
                job.Status = JobStatus.InContract;

            contract.AddDomainEvent(new ContractSignedEvent(contract.Id, freelancer.Id));

            /* 3) Коммит всех изменений разом */
            await db.SaveChangesAsync();
            //await trx.CommitAsync();

            return CommandResult.Success(contract.Id);
        }
    }

}
