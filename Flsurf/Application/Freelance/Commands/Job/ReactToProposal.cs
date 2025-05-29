using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class ReactToProposalCommand : BaseCommand {  
        public Guid ProposalId { get; set; }
        public ProposalStatus Reaction { get; set; }
    }

    public class ReactToProposalHandler(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<ReactToProposalCommand> 
    {
        public async Task<CommandResult> Handle(ReactToProposalCommand command)
        {
            var currentUser = await permService.GetCurrentUser();

            var proposal = await dbContext.Proposals
                .Include(x => x.Job)
                .FirstOrDefaultAsync(x => x.Id == command.ProposalId);

            if (proposal == null)
                return CommandResult.NotFound("Не найден", command.ProposalId);

            var job = await dbContext.Jobs
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == proposal.JobId);
            if (job == null)
                return CommandResult.NotFound("Не найден", proposal.JobId);

            proposal.Status = command.Reaction;

            proposal.AddDomainEvent(new ReactedToProposal(proposal.Id));

            await dbContext.SaveChangesAsync(); 

            return CommandResult.Success(proposal.Id); 
        }
    }
}
