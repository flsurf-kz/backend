using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Freelance.Commands.WorkSession
{
    public class EndWorkSessionCommand : BaseCommand
    {
        public Guid SessionId { get; set; }
        public List<CreateFileDto> SelectedFiles { get; set; } = [];
    }

    public class EndWorkSessionHandler(IApplicationDbContext _context, IPermissionService _permService, UploadFiles _uploadFiles)
        : ICommandHandler<EndWorkSessionCommand>
    {
        public async Task<CommandResult> Handle(EndWorkSessionCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var session = await _context.WorkSessions.Include(s => s.Files)
                             .FirstOrDefaultAsync(s => s.Id == command.SessionId);

            if (session == null)
                return CommandResult.NotFound("Work session not found", command.SessionId);

            if (session.FreelancerId != user.Id)
                return CommandResult.Forbidden();

            var uploadedFiles = await _uploadFiles.Execute(command.SelectedFiles);
            session.Files = uploadedFiles.ToList();
            session.EndSession();
            session.AddDomainEvent(new WorkSessionEnded(session, DateTime.Now));

            await _context.SaveChangesAsync();
            return CommandResult.Success(session.Id);
        }
    }

}
