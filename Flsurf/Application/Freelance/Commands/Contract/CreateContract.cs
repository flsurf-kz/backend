using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

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

            // 1. Подготовка ContractEntity
            var contract = new ContractEntity
            {
                EmployerId = employer.Id,
                FreelancerId = freelancer.Id,
                Budget = command.Budget,
                CostPerHour = command.CostPerHour,
                BudgetType = command.BudgetType,
                PaymentSchedule = command.PaymentSchedule,
                ContractTerms = command.ContractTerms,
                StartDate = DateTime.UtcNow,
                EndDate = command.EndDate,
                Status = ContractStatus.PendingApproval
            };

            _dbContext.Contracts.Add(contract);

            // 2. Изменение состояния JobEntity
            job.Contract = contract;
            job.Status = JobStatus.WaitingFreelancerApproval;

            // 3. Отправляем контракт на подтверждение фрилансеру
            contract.AddDomainEvent(new ContractSentToFreelancer(contract.Id, freelancer.Id));

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
