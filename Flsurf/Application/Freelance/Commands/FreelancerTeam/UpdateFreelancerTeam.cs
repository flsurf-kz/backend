using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.UseCases;
using Flsurf.Application.User.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerTeam
{
    public class UpdateFreelancerTeamCommand : BaseCommand
    {
        public Guid TeamId { get; set; }
        public string? Name { get; set; }
        public IFormFile? AvatarFile { get; set; }
        public bool? Closed { get; set; }
        public string? ClosedReason { get; set; }
    }


    public class UpdateFreelancerTeamHandler(IApplicationDbContext context, IPermissionService permService, UploadFile uploadFileUseCase)
        : ICommandHandler<UpdateFreelancerTeamCommand>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IPermissionService _permService = permService;
        private readonly UploadFile _uploadFileUseCase = uploadFileUseCase;

        public async Task<CommandResult> Handle(UpdateFreelancerTeamCommand command)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();

            // 🔎 Проверяем, существует ли команда
            var team = await _context.FreelancerTeams.FirstOrDefaultAsync(t => t.Id == command.TeamId);
            if (team == null)
            {
                return CommandResult.NotFound("Freelancer team not found.", command.TeamId);
            }

            // 🔒 Проверяем, является ли `user` владельцем команды
            bool isOwner = await _permService.CheckPermission(
                ZedGroup.WithId(team.Id).Resource, "owner", user.Id.ToString()
            );

            if (!isOwner)
            {
                return CommandResult.Forbidden("Only the owner can update the team.");
            }

            // 🔄 Обновляем название, если передано
            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                team.Name = command.Name;
            }

            // 📁 Обновляем аватар, если передан новый файл
            if (command.AvatarFile != null)
            {
                var uploadedFile = await _uploadFileUseCase.Execute(command.AvatarFile);
                team.AvatarId = uploadedFile.Id;
            }

            // 🚫 Закрытие команды (если передан флаг `Closed`)
            if (command.Closed.HasValue)
            {
                team.Closed = command.Closed.Value;
                if (team.Closed && !string.IsNullOrWhiteSpace(command.ClosedReason))
                {
                    team.ClosedReason = command.ClosedReason;
                }
            }

            // 💾 Сохраняем изменения в БД
            await _context.SaveChangesAsync();

            return CommandResult.Success(team.Id);
        }
    }

}
