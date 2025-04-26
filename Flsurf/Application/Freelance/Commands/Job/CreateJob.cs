using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.Models;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class CreateJobCommand : BaseCommand
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Guid> RequiredSkillIds { get; set; } = [];
        public Guid CategoryId { get; set; }
        [GreaterThanZero]
        public decimal? Budget { get; set; }
        [GreaterThanZero]
        public decimal? HourlyRate { get; set; }
        public int? Duration { get; set; }
        public BudgetType BudgetType { get; set; }
        public JobLevel Level { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public ICollection<CreateFileDto> Files { get; set; } = [];
    }

    // TODO FIX THIS
    public class CreateJobHandler(IApplicationDbContext dbContext, IPermissionService permService, UploadFiles uploadFiles) : ICommandHandler<CreateJobCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly UploadFiles _uploadFiles = uploadFiles;
        private readonly IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(CreateJobCommand command)
        {
            var user = await _permService.GetCurrentUser();


            if (user.Type != UserTypes.Client)
            {
                return CommandResult.Forbidden("Только клиенты могут создавать вакансии.");
            }

            // Загружаем связанные сущности
            var skills = await _dbContext.Skills
                .Where(s => command.RequiredSkillIds.Contains(s.Id))
                .ToListAsync();

            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == command.CategoryId);

            if (category == null)
            {
                return CommandResult.NotFound("", command.CategoryId);
            }

            var files = await _uploadFiles.Execute(command.Files);

            // Создаём вакансию
            JobEntity job = command.BudgetType == BudgetType.Fixed
                ? JobEntity.CreateFixed(
                    user, command.Title, command.Description, new Money(command.Budget ?? 0), false, command.Level, skills, files, category)
                : JobEntity.CreateHourly(
                    user, command.Title, command.Description, new Money(command.HourlyRate ?? 0), false, command.Level, skills, files, category);

            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();

            // Добавляем права доступа на эту вакансию
            await _permService.AddRelationship(
                ZedJob.WithId(job.Id).Owner(ZedUser.WithId(user.Id))
            );

            // Job is responsible for only Public view, and Contract is about after job 

            return CommandResult.Success(job.Id);
        }
    }

}
