using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class SubmitProposalCommand : BaseCommand
    {
        public Guid JobId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public decimal? ProposedRate { get; set; }
    }
    
    // TODO FIX THIS
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
                ProposedRate = command.ProposedRate ?? 0,
                Status = ProposalStatus.Pending
            };

            _dbContext.Proposals.Add(proposal);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(proposal.Id);
        }
    }

}
