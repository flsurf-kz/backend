using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.WorkSession
{
    // TODO!! Перенаправление события о одобрении в пеймент модуль и там необходима обработка 
    // и создания транзакции и вычесления из кошелка клиента 
    public class ApproveWorkSessionCommand : BaseCommand
    {
        public Guid SessionId { get; set; }
    }

    public class ApproveWorkSessionHandler(IApplicationDbContext _context, IPermissionService _permService)
        : ICommandHandler<ApproveWorkSessionCommand>
    {
        public async Task<CommandResult> Handle(ApproveWorkSessionCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var session = await _context.WorkSessions.Include(s => s.Contract)
                             .FirstOrDefaultAsync(s => s.Id == command.SessionId);

            if (session == null)
                return CommandResult.NotFound("Work session not found", command.SessionId);

            if (session.Contract.EmployerId != user.Id)
                return CommandResult.Forbidden("");

            session.AddDomainEvent(new WorkSessionApproved(session, DateTime.Now));

            await _context.SaveChangesAsync();
            return CommandResult.Success(session.Id);
        }
    }

}
