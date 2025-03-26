using Flsurf.Application.Freelance.Commands.WorkSession;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class WorkSessionService : IWorkSessionService
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkSessionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ApproveWorkSessionHandler ApproveWorkSession() =>
            _serviceProvider.GetRequiredService<ApproveWorkSessionHandler>();

        public EndWorkSessionHandler EndWorkSession() =>
            _serviceProvider.GetRequiredService<EndWorkSessionHandler>();

        public ReactToWorkSessionHandler ReactToWorkSession() =>
            _serviceProvider.GetRequiredService<ReactToWorkSessionHandler>();

        public StartWorkSessionHandler StartWorkSession() =>
            _serviceProvider.GetRequiredService<StartWorkSessionHandler>();

        public SubmitWorkSessionHandler SubmitWorkSession() =>
            _serviceProvider.GetRequiredService<SubmitWorkSessionHandler>();

        public GetWorkSessionsListHandler GetWorkSessionsList() =>
            _serviceProvider.GetRequiredService<GetWorkSessionsListHandler>();

        public GetWorkSessionHandler GetWorkSession() =>
            _serviceProvider.GetRequiredService<GetWorkSessionHandler>();
    }
}
