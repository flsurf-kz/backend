using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Commands.Contract
{
    public sealed class UpdateContractCommand : BaseCommand
    {
        [Required] public Guid ContractId { get; init; }

        // «незначительные» поля — все опциональные
        public PaymentScheduleType? PaymentSchedule { get; init; }
        public string? ContractTerms { get; init; }
        public DateTime? EndDate { get; init; }

        // работа с файлами
        public List<Guid>? AddFileIds { get; init; }
        public List<Guid>? RemoveFileIds { get; init; }
    }

    public sealed class UpdateContractHandler(
            IApplicationDbContext db,
            IPermissionService perm)
        : ICommandHandler<UpdateContractCommand>
    {
        public async Task<CommandResult> Handle(UpdateContractCommand cmd)
        {
            /* 1. Авторизация — обе стороны контракта могут редактировать «мелочи» */
            var user = await perm.GetCurrentUser();

            var contract = await db.Contracts
                .Include(c => c.Files)
                .FirstOrDefaultAsync(c => c.Id == cmd.ContractId);

            if (contract is null)
                return CommandResult.NotFound("Контракт не найден", cmd.ContractId);

            if (user.Id != contract.EmployerId && user.Id != contract.FreelancerId)
                return CommandResult.Forbidden("НЕ надо");

            /* 2. Нельзя менять, если контракт отменён или завершён */
            if (contract.Status is ContractStatus.Completed
                             or ContractStatus.Cancelled)
                return CommandResult.Conflict("Контракт уже закрыт");

            /* 3. Обновляем ТОЛЬКО «незначительные» поля */
            bool changed = false;

            if (cmd.PaymentSchedule.HasValue &&
                cmd.PaymentSchedule.Value != contract.PaymentSchedule)
            {
                contract.PaymentSchedule = cmd.PaymentSchedule.Value;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(cmd.ContractTerms) &&
                cmd.ContractTerms != contract.ContractTerms)
            {
                contract.ContractTerms = cmd.ContractTerms!;
                changed = true;
            }

            if (cmd.EndDate.HasValue && cmd.EndDate != contract.EndDate)
            {
                contract.EndDate = cmd.EndDate;
                changed = true;
            }

            /* 4. Файлы */
            if (cmd.AddFileIds is { Count: > 0 })
            {
                var newFiles = await db.Files
                    .Where(f => cmd.AddFileIds.Contains(f.Id))
                    .ToListAsync();

                foreach (var f in newFiles.Where(f => !contract.Files.Any(cf => cf.Id == f.Id)))
                    contract.Files.Add(f);

                if (newFiles.Count > 0) changed = true;
            }

            if (cmd.RemoveFileIds is { Count: > 0 })
            {
                contract.Files.RemoveAll(f => cmd.RemoveFileIds.Contains(f.Id));
                changed = true;
            }

            if (!changed)
                return CommandResult.Success();      // ничего не изменилось

            /* 5. Событие и сохранение */
            contract.AddDomainEvent(new ContractUpdated(contract.Id));
            await db.SaveChangesAsync();

            return CommandResult.Success(contract.Id);
        }
    }

}
