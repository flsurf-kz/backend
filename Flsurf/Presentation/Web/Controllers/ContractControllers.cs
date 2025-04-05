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

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateContract([FromBody] CreateContractCommand command)
        {
            var handler = _contractService.CreateContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-accept-finish")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientAcceptFinishContract([FromBody] ClientAcceptFinishContractCommand command)
        {
            var handler = _contractService.ClientAcceptFinishContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-close")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientCloseContract([FromBody] ClientCloseContractCommand command)
        {
            var handler = _contractService.ClientCloseContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("client-reject-completion")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ClientRejectContractCompletion([FromBody] ClientRejectContractCompletionCommand command)
        {
            var handler = _contractService.ClientRejectContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("freelancer-accept")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> FreelancerAcceptContract([FromBody] FreelancerAcceptContractCommand command)
        {
            var handler = _contractService.FreelancerAcceptContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("freelancer-finish")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> FreelancerFinishContract([FromBody] FreelancerFinishContractCommand command)
        {
            var handler = _contractService.FreelancerFinishContract();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("accept-dispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> AcceptDispute([FromBody] AcceptDisputeCommand command)
        {
            var handler = _contractService.AcceptDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("initiate-dispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> InitiateDispute([FromBody] InitiateDisputeCommand command)
        {
            var handler = _contractService.InitiateDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("resolve-dispute")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ResolveDispute([FromBody] ResolveDisputeCommand command)
        {
            var handler = _contractService.ResolveDispute();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("force-cancel")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> ForceContractCancel([FromBody] ForceContractCancelCommand command)
        {
            var handler = _contractService.ForceContractCancel();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractEntity>> GetContract(Guid id)
        {
            var query = new GetContractQuery { ContractId = id };
            var handler = _contractService.GetContract();
            var contract = await handler.Handle(query);
            if (contract == null)
                return NotFound("Контракт не найден");
            return Ok(contract);
        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<ContractEntity>>> GetContractsList(
            [FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetContractsListQuery { Start = start, Ends = end };
            var handler = _contractService.GetContractsList();
            var contracts = await handler.Handle(query);
            return Ok(contracts);
        }
    }
}
