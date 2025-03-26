using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Staff.Interfaces;
using Flsurf.Application.Staff.UseCases;
using Flsurf.Domain.Freelance;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public class InitiateDisputeCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class InitiateDisputeHandler(
    IApplicationDbContext dbContext,
    IPermissionService permService,
    IStaffService staffService)
    : ICommandHandler<InitiateDisputeCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;
        private readonly IStaffService _staffService = staffService;

        public async Task<CommandResult> Handle(InitiateDisputeCommand command)
        {
            var initiator = await _permService.GetCurrentUser();

            var contract = await _dbContext.Contracts
                .Include(c => c.Freelancer)
                .Include(c => c.Employer)
                .Include(c => c.WorkSessions)
                .FirstOrDefaultAsync(c => c.Id == command.ContractId);

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден", command.ContractId);

            if (contract.Status != ContractStatus.Active)
                return CommandResult.BadRequest("Спор возможен только для активных контрактов.");

            // Проверка, является ли текущий пользователь стороной контракта
            if (initiator.Id != contract.EmployerId && initiator.Id != contract.FreelancerId)
                return CommandResult.Forbidden("Вы не являетесь участником контракта.");

            // Проверка, что спора ещё нет
            var existingDispute = await _dbContext.Disputes.FirstOrDefaultAsync(d => d.ContractId == contract.Id && d.Status != DisputeStatus.Resolved);
            if (existingDispute != null)
                return CommandResult.BadRequest("По данному контракту уже есть открытый спор.");

            // Создание сущности спора
            var dispute = new DisputeEntity
            {
                ContractId = contract.Id,
                InitiatorId = initiator.Id,
                Reason = command.Reason,
                Status = DisputeStatus.Pending,
            };

            _dbContext.Disputes.Add(dispute);

            // Рассчитываем приоритет
            double priorityScore = PriorityCalculator.CalculatePriorityScore(
                contractAmountUsd: (double)(contract.Budget ?? (contract.CostPerHour ?? 0) * 2),
                clientHasPremium: false,
                freelancerHasPremium: false,
                freelancerIsPopular: false,
                clientIsBigCompany: true
            );

            // Создание тикета в Staff-контексте
            var ticket = await _staffService.CreateTicket().Execute(
                new Staff.Dto.CreateTicketDto() { 
                    Subject = "Жалоба контракт",
                    Files = [], 
                    LinkedDisputeId = dispute.Id, 
                    PriorityScore = priorityScore, 
                    Text = "Жалоба на контракте", 
                }
            );

            dispute.StaffTicketId = ticket.Id;

            // Событие создания спора
            dispute.AddDomainEvent(new DisputeInitiatedEvent(dispute.Id, ticket.Id, contract.Id));

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(dispute.Id);
        }
    }

}
