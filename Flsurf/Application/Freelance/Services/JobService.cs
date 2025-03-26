using Flsurf.Application.Freelance.Commands.Job;
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

        public CreateJobCommand CreateJob() =>
            _serviceProvider.GetRequiredService<CreateJobCommand>();

        public UpdateJobCommand UpdateJob() =>
            _serviceProvider.GetRequiredService<UpdateJobCommand>();

        public DeleteJobCommand DeleteJob() =>
            _serviceProvider.GetRequiredService<DeleteJobCommand>();

        public GetJobQuery GetJob() =>
            _serviceProvider.GetRequiredService<GetJobQuery>();

        public GetJobsListQuery GetJobsList() =>
            _serviceProvider.GetRequiredService<GetJobsListQuery>();

        public BookmarkJobHandler BookmarkJob() =>
            _serviceProvider.GetRequiredService<BookmarkJobHandler>();

        public GetClientOrderInfoHandler GetClientOrderInfo() =>
            _serviceProvider.GetRequiredService<GetClientOrderInfoHandler>(); 

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
    }
}
