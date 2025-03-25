using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class ReactToContestEntryCommand : BaseCommand
    {
        public Guid ContestEntryId { get; set; }
        public ContestEntryReaction Reaction { get; set; }
    }

    public class ReactToContestEntryHandler : ICommandHandler<ReactToContestEntryCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public ReactToContestEntryHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(ReactToContestEntryCommand command)
        {
            // Получаем текущего пользователя
            var currentUser = await _permService.GetCurrentUser();

            // Проверяем, существует ли заявка конкурса
            var contestEntry = await _context.ContestEntries
                .FirstOrDefaultAsync(e => e.Id == command.ContestEntryId);

            if (contestEntry == null)
            {
                return CommandResult.NotFound("Заявка на конкурс не найдена.", command.ContestEntryId);
            }

            contestEntry.Reaction = command.Reaction; 

            await _context.SaveChangesAsync();

            return CommandResult.Success(command.ContestEntryId);
        }
    }
}
