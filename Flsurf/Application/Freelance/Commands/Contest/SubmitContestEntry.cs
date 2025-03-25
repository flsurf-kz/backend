using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class SubmitContestEntryCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<CreateFileDto> SubmissionFiles { get; set; } = [];
    }

    public class SubmitContestEntryHandler : ICommandHandler<SubmitContestEntryCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly UploadFiles _uploadFiles; 

        public SubmitContestEntryHandler(
            IApplicationDbContext context,
            IPermissionService permService, 
            UploadFiles uploadFiles)
        {
            _uploadFiles = uploadFiles; 
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(SubmitContestEntryCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            // Проверяем, является ли пользователь фрилансером
            if (currentUser.Type != UserTypes.Freelancer)
                return CommandResult.Forbidden("Только фрилансеры могут отправлять работы на конкурс.");

            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.Id == command.ContestId);

            if (contest == null)
                return CommandResult.NotFound("Конкурс не найден.", command.ContestId);

            if (contest.Status != ContestStatus.Open)
                return CommandResult.BadRequest("Конкурс не открыт для приема работ.");

            // Проверяем, не отправлял ли фрилансер уже работу на этот конкурс
            var existingEntry = await _context.ContestEntries
                .FirstOrDefaultAsync(e => e.ContestId == command.ContestId && e.FreelancerId == currentUser.Id);

            if (existingEntry != null)
                return CommandResult.BadRequest("Вы уже отправили работу на этот конкурс.");

            var files = await _uploadFiles.Execute(command.SubmissionFiles);

            // Создание заявки
            var contestEntry = new ContestEntryEntity
            {
                Id = Guid.NewGuid(),
                ContestId = command.ContestId,
                FreelancerId = currentUser.Id,
                Files = files,
                Description = command.Description
            };

            _context.ContestEntries.Add(contestEntry);

            await _context.SaveChangesAsync();

            return CommandResult.Success(contestEntry.Id);
        }
    }
}
