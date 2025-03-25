using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class DeleteContestEntryCommand : BaseCommand
    {
        public Guid ContestEntryId { get; set; }
    }

    public class DeleteContestEntryHandler : ICommandHandler<DeleteContestEntryCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeleteContestEntryHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeleteContestEntryCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();
            var entry = await _context.ContestEntries.FirstOrDefaultAsync(e => e.Id == command.ContestEntryId);
            if (entry == null)
                return CommandResult.NotFound("Заявка на конкурс не найдена.", command.ContestEntryId);
            if (entry.FreelancerId != currentUser.Id)
                return CommandResult.Forbidden("Вы не можете удалить чужую заявку.");

            _context.ContestEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return CommandResult.Success(entry.Id);
        }
    }
}
