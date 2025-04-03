﻿using Flsurf.Application.Freelance.Commands.Job;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    // Job + Bookmarks
    public interface IJobService
    {
        CreateJobHandler CreateJob();
        UpdateJobHandler UpdateJob();
        DeleteJobHandler DeleteJob();
        GetJobHandler GetJob();
        GetJobsListHandler GetJobsList();
        GetClientOrderInfoHandler GetClientOrderInfo(); // если нужно
        BookmarkJobHandler BookmarkJob(); 
        HideJobHandler HideJob();
        SubmitProposalHandler SubmitProposal(); 
        UpdateProposalHandler UpdateProposal();
        WithdrawProposalHandler WithdrawProposal();
        GetBookmarksListHandler GetBookmarksList(); // если закладки относятся к вакансиям
    }
}
