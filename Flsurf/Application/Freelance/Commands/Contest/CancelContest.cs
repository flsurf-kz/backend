using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class CancelContestCommand : BaseCommand
    {
        public Guid ContestId { get; set; }
        public string Reason { get; set; } = null!; 
    }

    public class CancelContest(IApplicationDbContext dbContext, IPermissionService _permService) : ICommandHandler<CancelContestCommand>
    {
        public IApplicationDbContext _dbContext = dbContext; 
        public IPermissionService _permService = _permService;

        public async Task<CommandResult> Handle(CancelContestCommand command)
        {

        }
    }
}
