using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class ReactToContestEntryCommand : BaseCommand
    {
        public Guid ContestEntryId { get; set; }
        public string Reaction { get; set; } = string.Empty;
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

            // Ищем существующую реакцию от текущего пользователя на эту заявку
            var existingReaction = await _context.ContestEntryReactions
                .FirstOrDefaultAsync(r => r.ContestEntryId == command.ContestEntryId && r.UserId == currentUser.Id);

            if (existingReaction != null)
            {
                // Если реакция уже существует, обновляем её
                existingReaction.Reaction = command.Reaction;
                existingReaction.ReactedAt = DateTime.UtcNow;
            }
            else
            {
                // Если реакции нет, создаём новую сущность реакции
                var reactionEntity = new ContestEntryReactionEntity
                {
                    Id = Guid.NewGuid(),
                    ContestEntryId = command.ContestEntryId,
                    UserId = currentUser.Id,
                    Reaction = command.Reaction,
                    ReactedAt = DateTime.UtcNow
                };

                _context.ContestEntryReactions.Add(reactionEntity);
            }

            await _context.SaveChangesAsync();

            return CommandResult.Success(command.ContestEntryId);
        }
    }
}
