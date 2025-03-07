using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class CreateFreelancerTeamHandler : ICommandHandler<CreateFreelancerTeamCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public CreateFreelancerTeamHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateFreelancerTeamCommand command)
        {
            // Заглушка: ничего не делаем, просто имитируем асинхронную операцию
            await Task.CompletedTask;

            return CommandResult.Success(); // Возвращает успешный результат без данных
        }
    }
}
