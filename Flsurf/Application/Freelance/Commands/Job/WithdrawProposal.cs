using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class WithdrawProposalCommand : BaseCommand
    {
        public Guid ProposalId { get; set; }
    }

    public class WithdrawProposalHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<WithdrawProposalCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permissionService = permService;

        public async Task<CommandResult> Handle(WithdrawProposalCommand command)
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
                return CommandResult.Forbidden("You can only withdraw your own proposals.");
            }

            // Нельзя отозвать предложение, если оно уже принято
            if (proposal.Status == ProposalStatus.Accepted)
            {
                return CommandResult.Conflict("Cannot withdraw a proposal that has already been accepted.");
            }

            _dbContext.Proposals.Remove(proposal);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(proposal.Id);
        }
    }

}
