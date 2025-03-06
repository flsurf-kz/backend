using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.WorkSession
{
    public class StartWorkSessionCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
    }

    public class StartWorkSessionHandler(IApplicationDbContext _context, IPermissionService _permService)
        : ICommandHandler<StartWorkSessionCommand>
    {
        public async Task<CommandResult> Handle(StartWorkSessionCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == command.ContractId);

            if (contract == null)
                return CommandResult.NotFound("Contract not found", command.ContractId);

            if (contract.FreelancerId != user.Id)
                return CommandResult.Forbidden("");

            var session = WorkSessionEntity.Create(user, contract);
            _context.WorkSessions.Add(session);
            await _context.SaveChangesAsync();

            session.AddDomainEvent(new WorkSessionStarted(session, session.StartDate));

            return CommandResult.Success(session.Id);
        }
    }

}
