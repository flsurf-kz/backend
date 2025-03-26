using Flsurf.Application.Freelance.Commands.Contest;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class ContestService : IContestService
    {
        private readonly IServiceProvider _serviceProvider;

        public ContestService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateContestHandler CreateContest() =>
            _serviceProvider.GetRequiredService<CreateContestHandler>();

        public ApproveContestHandler ApproveContest() =>
            _serviceProvider.GetRequiredService<ApproveContestHandler>();

        public StartContestHandler StartContest() =>
            _serviceProvider.GetRequiredService<StartContestHandler>();

        public EndContestHandler EndContest() =>
            _serviceProvider.GetRequiredService<EndContestHandler>();

        public DeleteContestHandler DeleteContest() =>
            _serviceProvider.GetRequiredService<DeleteContestHandler>();

        public SelectContestWinnerHandler SelectContestWinner() =>
            _serviceProvider.GetRequiredService<SelectContestWinnerHandler>();

        public SubmitContestEntryHandler SubmitContestEntry() =>
            _serviceProvider.GetRequiredService<SubmitContestEntryHandler>();

        public DeleteContestEntryHandler DeleteContestEntry() =>
            _serviceProvider.GetRequiredService<DeleteContestEntryHandler>();

        public UpdateContestHandler UpdateContest() =>
            _serviceProvider.GetRequiredService<UpdateContestHandler>();

        public GetContestHandler GetContest() =>
            _serviceProvider.GetRequiredService<GetContestHandler>();

        public GetContestListHandler GetContestList() =>
            _serviceProvider.GetRequiredService<GetContestListHandler>();
    }
}
