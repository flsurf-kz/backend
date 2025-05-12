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
using System.Diagnostics.Contracts;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class CreateContractCommand : BaseCommand
    {
        public Guid FreelancerId { get; set; }
        public Guid JobId { get; set; }
        public decimal? Budget { get; set; }
        public decimal? CostPerHour { get; set; }
        public BudgetType BudgetType { get; set; }
        public PaymentScheduleType PaymentSchedule { get; set; }
        public string ContractTerms { get; set; } = string.Empty;
        public DateTime? EndDate { get; set; }
    }

    public class CreateContractHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)
        : ICommandHandler<CreateContractCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(CreateContractCommand command)
        {
            var employer = await _permService.GetCurrentUser();

            if (employer.Type != UserTypes.Client)
                return CommandResult.Forbidden("Только клиенты могут создавать контракты.");

            var freelancer = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == command.FreelancerId && x.Type == UserTypes.Freelancer);

            if (freelancer == null)
                return CommandResult.NotFound("Фрилансер не найден.", command.FreelancerId);

            var job = await _dbContext.Jobs
                .Include(j => j.Contract)
                .FirstOrDefaultAsync(x => x.Id == command.JobId && x.EmployerId == employer.Id);

            if (job == null)
                return CommandResult.NotFound("Вакансия не найдена или недоступна.", command.JobId);

            if (job.Contract != null)
                return CommandResult.BadRequest("У этой вакансии уже есть контракт.");

            if (job.Status != JobStatus.Open)
                return CommandResult.BadRequest("Работа не открыта"); 

            ContractEntity contract;

            if (command.BudgetType == BudgetType.Fixed)
            {
                if (command.Budget == null)
                    return CommandResult.BadRequest("Не указан бюджет для фиксированного контракта.");

                contract = ContractEntity.CreateFixed(
                    employerId: employer.Id,
                    freelancerId: freelancer.Id,
                    budget: command.Budget,
                    paymentSchedule: command.PaymentSchedule,
                    contractTerms: command.ContractTerms,
                    endDate: command.EndDate, 
                    jobId: job.Id
                );
            }
            else if (command.BudgetType == BudgetType.Hourly)
            {
                if (command.CostPerHour == null)
                    return CommandResult.BadRequest("Не указана ставка для почасового контракта.");

                contract = ContractEntity.CreateHourly(
                    employerId: employer.Id,
                    freelancerId: freelancer.Id,
                    costPerHour: (decimal)command.CostPerHour,
                    paymentSchedule: command.PaymentSchedule,
                    contractTerms: command.ContractTerms,
                    endDate: command.EndDate, 
                    jobId: job.Id 
                );
            }
            else
            {
                return CommandResult.BadRequest("Неизвестный тип бюджета.");
            }

            _dbContext.Contracts.Add(contract);

            // 2. Изменение состояния JobEntity
            job.Contract = contract;
            job.Status = JobStatus.WaitingFreelancerApproval;

            // 3. Отправляем контракт на подтверждение фрилансеру
            contract.AddDomainEvent(new ContractWasCreated(contract, job));

            await _dbContext.SaveChangesAsync();

            // Авторизация через ReBAC/AuthZed
            await _permService.AddRelationship(
                ZedContract.WithId(contract.Id).Client(ZedFreelancerUser.WithId(employer.Id)));
            await _permService.AddRelationship(
                ZedContract.WithId(contract.Id).Freelancer(ZedFreelancerUser.WithId(freelancer.Id)));

            // Остальные операции (оплаты и заморозки) произойдут после подтверждения фрилансером.

            return CommandResult.Success(contract.Id);
        }
    }

}
