using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.WorkSession
{
    public class SubmitWorkSessionCommand : BaseCommand
    {
        public Guid SessionId { get; set; }
    }

    public class SubmitWorkSessionHandler(IApplicationDbContext _context, IPermissionService _permService)
        : ICommandHandler<SubmitWorkSessionCommand>
    {
        public async Task<CommandResult> Handle(SubmitWorkSessionCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var session = await _context.WorkSessions.FirstOrDefaultAsync(s => s.Id == command.SessionId);

            if (session == null)
                return CommandResult.NotFound("Work session not found", command.SessionId);

            if (session.FreelancerId != user.Id)
                return CommandResult.Forbidden("");

            session.AddDomainEvent(new WorkSessionSubmitted(session, DateTime.Now));

            await _context.SaveChangesAsync();
            return CommandResult.Success(session.Id);
        }
    }

}
