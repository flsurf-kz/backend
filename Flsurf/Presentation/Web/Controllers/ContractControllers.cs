using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Freelance.Commands.Contract;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/contract")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost("create", Name = "CreateContract")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateContract([FromBody] CreateContractCommand command)
        {
            var handler = _contractService.CreateContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-accept-finish", Name = "ClientAcceptFinishContract")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientAcceptFinishContract([FromBody] ClientAcceptFinishContractCommand command)
        {
            var handler = _contractService.ClientAcceptFinishContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-close", Name = "ClientCloseContract")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientCloseContract([FromBody] ClientCloseContractCommand command)
        {
            var handler = _contractService.ClientCloseContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-reject-completion", Name = "ClientRejectContractCompletion")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientRejectContractCompletion([FromBody] ClientRejectContractCompletionCommand command)
        {
            var handler = _contractService.ClientRejectContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("freelancer-accept", Name = "FreelancerAcceptContract")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> FreelancerAcceptContract([FromBody] FreelancerAcceptContractCommand command)
        {
            var handler = _contractService.FreelancerAcceptContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("freelancer-finish", Name = "FreelancerFinishContract")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> FreelancerFinishContract([FromBody] FreelancerFinishContractCommand command)
        {
            var handler = _contractService.FreelancerFinishContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("accept-dispute", Name = "AcceptDispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> AcceptDispute([FromBody] AcceptDisputeCommand command)
        {
            var handler = _contractService.AcceptDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("initiate-dispute", Name = "InitiateDispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> InitiateDispute([FromBody] InitiateDisputeCommand command)
        {
            var handler = _contractService.InitiateDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("resolve-dispute", Name = "ResolveDispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ResolveDispute([FromBody] ResolveDisputeCommand command)
        {
            var handler = _contractService.ResolveDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("force-cancel", Name = "ForceContractCancel")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ForceContractCancel([FromBody] ForceContractCancelCommand command)
        {
            var handler = _contractService.ForceContractCancel();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}", Name = "GetContract")]
        public async Task<ActionResult<ContractEntity>> GetContract(Guid id)
        {
            var query = new GetContractQuery { ContractId = id };
            var handler = _contractService.GetContract();
            var contract = await handler.Handle(query);
            if (contract == null)
                return NotFound("Контракт не найден");
            return Ok(contract);
        }

        [HttpPost("list", Name = "GetContractsList")]
        public async Task<ActionResult<ICollection<ContractEntity>>> GetContractsList([FromBody] GetContractsListQuery query)
        {
            var handler = _contractService.GetContractsList();
            var contracts = await handler.Handle(query);
            return Ok(contracts);
        }
    }
}
