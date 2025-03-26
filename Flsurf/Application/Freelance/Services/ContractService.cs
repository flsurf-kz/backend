using Flsurf.Application.Freelance.Commands.Contract;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class ContractService : IContractService
    {
        private readonly IServiceProvider _serviceProvider;

        public ContractService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public AcceptDisputeHandler AcceptDispute() =>
            _serviceProvider.GetRequiredService<AcceptDisputeHandler>();

        public ClientAcceptFinishContractHandler ClientAcceptFinishContract() =>
            _serviceProvider.GetRequiredService<ClientAcceptFinishContractHandler>();

        public ClientCloseContractHandler ClientCloseContract() =>
            _serviceProvider.GetRequiredService<ClientCloseContractHandler>();

        public ClientRejectContractCompletionHandler ClientRejectContract() =>
            _serviceProvider.GetRequiredService<ClientRejectContractCompletionHandler>();

        public CreateContractHandler CreateContract() =>
            _serviceProvider.GetRequiredService<CreateContractHandler>();

        public ForceContractCancelHandler ForceContractCancel() =>
            _serviceProvider.GetRequiredService<ForceContractCancelHandler>();

        public FreelancerAcceptContractHandler FreelancerAcceptContract() =>
            _serviceProvider.GetRequiredService<FreelancerAcceptContractHandler>();

        public FreelancerFinishContractHandler FreelancerFinishContract() =>
            _serviceProvider.GetRequiredService<FreelancerFinishContractHandler>();

        public InitiateDisputeHandler InitiateDispute() =>
            _serviceProvider.GetRequiredService<InitiateDisputeHandler>();

        public ResolveDisputeHandler ResolveDispute() =>
            _serviceProvider.GetRequiredService<ResolveDisputeHandler>();

        //public UpdateContractHandler UpdateContract() =>
        //    _serviceProvider.GetRequiredService<UpdateContractHandler>();

        public GetContractHandler GetContract() =>
            _serviceProvider.GetRequiredService<GetContractHandler>();

        public GetContractsListHandler GetContractsList() =>
            _serviceProvider.GetRequiredService<GetContractsListHandler>();
    }
}
