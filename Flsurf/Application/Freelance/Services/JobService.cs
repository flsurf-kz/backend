﻿using Flsurf.Application.Freelance.Commands.Job;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class JobService : IJobService
    {
        private readonly IServiceProvider _serviceProvider;

        public JobService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateJobHandler CreateJob() =>
            _serviceProvider.GetRequiredService<CreateJobHandler>();

        public UpdateJobHandler UpdateJob() =>
            _serviceProvider.GetRequiredService<UpdateJobHandler>();

        public DeleteJobHandler DeleteJob() =>
            _serviceProvider.GetRequiredService<DeleteJobHandler>();

        public GetJobHandler GetJob() =>
            _serviceProvider.GetRequiredService<GetJobHandler>();

        public GetRawJobHandler GetRawJob() =>
            _serviceProvider.GetRequiredService<GetRawJobHandler>(); 

        public GetJobsListHandler GetJobsList() =>
            _serviceProvider.GetRequiredService<GetJobsListHandler>();

        public BookmarkJobHandler BookmarkJob() =>
            _serviceProvider.GetRequiredService<BookmarkJobHandler>();

        public GetClientOrderInfoHandler GetClientOrderInfo() =>
            _serviceProvider.GetRequiredService<GetClientOrderInfoHandler>();

        public DislikeJobHandler DislikeJob() =>
            _serviceProvider.GetRequiredService<DislikeJobHandler>();

        public HideJobHandler HideJob() =>
            _serviceProvider.GetRequiredService<HideJobHandler>();

        public SubmitProposalHandler SubmitProposal() =>
            _serviceProvider.GetRequiredService<SubmitProposalHandler>();

        public UpdateProposalHandler UpdateProposal() =>
            _serviceProvider.GetRequiredService<UpdateProposalHandler>();

        public WithdrawProposalHandler WithdrawProposal() =>
            _serviceProvider.GetRequiredService<WithdrawProposalHandler>();

        public GetBookmarksListHandler GetBookmarksList() =>
            _serviceProvider.GetRequiredService<GetBookmarksListHandler>();

        public ReactToSentJobHandler ReactToSentJob() => 
            _serviceProvider.GetRequiredService<ReactToSentJobHandler>();

        public SendDraftJobToModerationHandler SendDraftJobToModeration() => 
            _serviceProvider.GetRequiredService<SendDraftJobToModerationHandler>();

        public ReactToProposalHandler ReactToProposal() =>
            _serviceProvider.GetRequiredService<ReactToProposalHandler>(); 

        public GetProposalsListHandler GetProposalsList() => 
            _serviceProvider.GetRequiredService<GetProposalsListHandler>();

        public StartChatWithFreelancerHandler StartChatWithFreelancer() => 
            _serviceProvider.GetRequiredService<StartChatWithFreelancerHandler>();
    }
}
