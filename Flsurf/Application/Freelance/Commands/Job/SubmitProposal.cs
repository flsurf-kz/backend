using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class SubmitProposalCommand : BaseCommand
    {
        [Required]
        public Guid JobId { get; set; }
        [Required]
        public string CoverLetter { get; set; } = string.Empty;
        [Required]
        public BudgetType BudgetType { get; set; }
        public decimal? ProposedRate { get; set; }
        public ICollection<CreateFileDto>? Files { get; set; }
        public int EstimatedDurationDays { get; set; }
        public string? SimilarExpirence { get; set; }
        public ICollection<Guid>? PortfolioProjectsIds { get; set; }
    }

    public class SubmitProposalHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<SubmitProposalCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(SubmitProposalCommand command)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Proposals) // Загружаем связанные предложения
                .FirstOrDefaultAsync(j => j.Id == command.JobId);

            if (job == null)
            {
                return CommandResult.NotFound("Job not found", command.JobId);
            }

            var user = await _permissionService.GetCurrentUser();

            if (user.Type != Domain.User.Enums.UserTypes.Freelancer)
            {
                return CommandResult.Forbidden("You do not have permission to submit a proposal.");
            }

            // Проверяем, открыт ли статус работы
            if (job.Status != JobStatus.Open)
            {
                return CommandResult.Conflict("Cannot submit a proposal. Job is not open.");
            }

            // Проверяем, не отправлял ли пользователь уже ставку
            if (job.Proposals.Any(p => p.FreelancerId == user.Id))
            {
                return CommandResult.BadRequest("You have already submitted a proposal for this job.");
            }

            var proposal = new ProposalEntity
            {
                FreelancerId = user.Id,
                Freelancer = user,
                JobId = job.Id,
                Job = job,
                CoverLetter = command.CoverLetter,
                ProposedRate = new Money(command.ProposedRate ?? 0),
                BudgetType = command.BudgetType, 
                Status = ProposalStatus.Pending, 
                EsitimatedDurationDays = command.EstimatedDurationDays, 
                SimilarExpriences = command.SimilarExpirence, 
                PortfolioProjects = command.PortfolioProjectsIds != null 
                    ? await _dbContext.PortfolioProjects.Where(x => command.PortfolioProjectsIds.Contains(x.Id)).ToListAsync() 
                    : null,
            };

            _dbContext.Proposals.Add(proposal);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(proposal.Id);
        }
    }

}
