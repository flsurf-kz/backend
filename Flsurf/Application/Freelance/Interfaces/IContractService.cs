using Flsurf.Application.Freelance.Commands.Contract;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IContractService
    {
        // Команды
        AcceptDisputeHandler AcceptDispute();
        ClientAcceptFinishContractHandler ClientAcceptFinishContract();
        ClientCloseContractHandler ClientCloseContract();
        ClientRejectContractCompletionHandler ClientRejectContract();
        CreateContractHandler CreateContract();
        ForceContractCancelHandler ForceContractCancel();
        FreelancerAcceptContractHandler FreelancerAcceptContract();
        FreelancerFinishContractHandler FreelancerFinishContract();
        InitiateDisputeHandler InitiateDispute();
        ResolveDisputeHandler ResolveDispute();
        //UpdateContractHandler UpdateContract();

        // Запросы
        GetContractHandler GetContract();
        GetContractsListHandler GetContractsList();
    }
}
