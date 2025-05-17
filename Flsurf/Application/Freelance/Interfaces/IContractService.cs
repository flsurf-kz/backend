// --- Flsurf.Application.Freelance.Interfaces.IContractService.cs ---
using Flsurf.Application.Freelance.Commands.Contract;
using Flsurf.Application.Freelance.Queries; // For GetBonusesForContractHandler

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

        // New Bonus Command Handler
        AddBonusToContractHandler AddBonusToContract(); // << NEW

        // Запросы
        GetContractHandler GetContract();
        GetContractsListHandler GetContractsList();

        // New Bonus Query Handler
        GetBonusesForContractHandler GetBonusesForContract(); // << NEW
    }
}