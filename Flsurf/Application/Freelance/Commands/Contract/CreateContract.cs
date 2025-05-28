using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class CreateContractCommand : BaseCommand
    {
        [Required]
        public Guid ProposalId { get; set; }

        // эти поля в Proposal’е не хранятся, их заполняет клиент-сервис при найме
        [Required]
        public PaymentScheduleType PaymentSchedule { get; set; }
        [Required]
        public string ContractTerms { get; set; } = string.Empty;
        public DateTime? EndDate { get; set; }
    }

    public class CreateContractHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)          // ReBAC / SpiceDB
            : ICommandHandler<CreateContractCommand>
    {
        private readonly IApplicationDbContext _db = dbContext;
        private readonly IPermissionService _perm = permService;

        public async Task<CommandResult> Handle(CreateContractCommand cmd)
        {
            /* 1. Автор   — только клиент */
            var employer = await _perm.GetCurrentUser();
            if (employer.Type != UserTypes.Client)
                return CommandResult.Forbidden("Только клиенты могут создавать контракты.");

            /* 2. Тянем Proposal + Job + Freelancer */
            var proposal = await _db.Proposals
                .Include(p => p.Job)
                .FirstOrDefaultAsync(p => p.Id == cmd.ProposalId);

            if (proposal is null)
                return CommandResult.NotFound("Оффер не найден.", cmd.ProposalId);

            var job = proposal.Job!;                    // благодаря Include
            var freelancerId = proposal.FreelancerId;

            /* 3. Проверки владельца и статусов */
            if (job.EmployerId != employer.Id)
                return CommandResult.Forbidden("Вы не владелец этой вакансии.");

            if (proposal.Status != ProposalStatus.Accepted)
                return CommandResult.BadRequest("Оффер должен быть в статусе Accepted.");

            if (job.Contract is not null)
                return CommandResult.BadRequest("Для этой вакансии уже есть контракт.");

            if (job.Status != JobStatus.Open)
                return CommandResult.BadRequest("Вакансия должна быть открыта.");

            /* 4. Единственная точка создания */
            ContractEntity contract = proposal.BudgetType switch
            {
                BudgetType.Fixed => ContractEntity.CreateFixed(
                    proposalId: proposal.Id,                 // 🔸
                    jobId: job.Id,
                    employerId: employer.Id,
                    freelancerId: freelancerId,
                    budget: proposal.ProposedRate,       // 🔸
                    paymentSchedule: cmd.PaymentSchedule,
                    contractTerms: cmd.ContractTerms,
                    endDate: cmd.EndDate
                ),

                BudgetType.Hourly => ContractEntity.CreateHourly(
                    proposalId: proposal.Id,                 // 🔸
                    jobId: job.Id,
                    employerId: employer.Id,
                    freelancerId: freelancerId,
                    costPerHour: proposal.ProposedRate,       // 🔸
                    paymentSchedule: cmd.PaymentSchedule,
                    contractTerms: cmd.ContractTerms,
                    endDate: cmd.EndDate
                ),

                _ => throw new ArgumentOutOfRangeException(nameof(proposal.BudgetType))
            };

            /* 5. Сохранение и изменение состояний */
            _db.Contracts.Add(contract);

            job.Contract = contract;
            job.Status = JobStatus.WaitingFreelancerApproval;
            proposal.Status = ProposalStatus.Accepted;         // новый статус

            contract.AddDomainEvent(new ContractWasCreated(contract, job));
            await _db.SaveChangesAsync();

            /* 6. ReBAC-отношения */
            await _perm.AddRelationship(
                ZedContract.WithId(contract.Id).Client(ZedFreelancerUser.WithId(employer.Id)));
            await _perm.AddRelationship(
                ZedContract.WithId(contract.Id).Freelancer(ZedFreelancerUser.WithId(freelancerId)));

            return CommandResult.Success(contract.Id);
        }
    }
}
