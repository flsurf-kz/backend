using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class UpdateProposalCommand : BaseCommand
    {
        public Guid ProposalId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public decimal? ProposedRate { get; set; }
    }


    public class UpdateProposalHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<UpdateProposalCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(UpdateProposalCommand command)
        {
            var proposal = await _dbContext.Proposals
                .Include(p => p.Job)
                .FirstOrDefaultAsync(p => p.Id == command.ProposalId);

            if (proposal == null)
            {
                return CommandResult.NotFound("Proposal not found", command.ProposalId);
            }

            var user = await _permissionService.GetCurrentUser();

            // Проверяем, является ли пользователь автором предложения
            if (proposal.FreelancerId != user.Id)
            {
                return CommandResult.Forbidden("You can only update your own proposals.");
            }

            // Нельзя редактировать предложение, если оно уже принято
            if (proposal.Status == ProposalStatus.Accepted)
            {
                return CommandResult.Conflict("Cannot update a proposal that has already been accepted.");
            }

            proposal.CoverLetter = command.CoverLetter;
            proposal.ProposedRate = new Money(command.ProposedRate ?? 0);

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(proposal.Id);
        }
    }

}
