using Flsurf.Application.Freelance.Commands.Job;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    // Job + Bookmarks
    public interface IJobService
    {
        CreateJobCommand CreateJob();
        UpdateJobCommand UpdateJob();
        DeleteJobCommand DeleteJob();
        GetJobQuery GetJob();
        GetJobsListQuery GetJobsList();
        GetClientOrderInfoHandler GetClientOrderInfo(); // если нужно
        BookmarkJobHandler BookmarkJob(); 
        HideJobHandler HideJob();
        SubmitProposalHandler SubmitProposal(); 
        UpdateProposalHandler UpdateProposal();
        WithdrawProposalHandler WithdrawProposal();
        GetBookmarksListHandler GetBookmarksList(); // если закладки относятся к вакансиям
    }
}
