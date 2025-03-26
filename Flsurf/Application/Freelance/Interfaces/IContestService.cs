using Flsurf.Application.Freelance.Commands.Contest;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IContestService
    {
        CreateContestHandler CreateContest();
        ApproveContestHandler ApproveContest();
        StartContestHandler StartContest();
        EndContestHandler EndContest();
        DeleteContestHandler DeleteContest();
        SelectContestWinnerHandler SelectContestWinner();
        SubmitContestEntryHandler SubmitContestEntry();
        DeleteContestEntryHandler DeleteContestEntry();
        UpdateContestHandler UpdateContest();
        GetContestHandler GetContest();
        GetContestListHandler GetContestList();
    }
}
