using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.Queries.Models;
using Flsurf.Application.Payment.UseCases;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IPermissionService _permService; 

        public WalletController(IWalletService walletService, IPermissionService permService)
        {
            _permService = permService; 
            _walletService = walletService;
        }

        /// <summary>
        /// Выполнение операции с балансом (например, пополнение, списание и т.д.).
        /// </summary>
        [HttpPost("balance-operation", Name = "BalanceOperation")]
        public async Task<ActionResult<CommandResult>> BalanceOperation([FromBody] BalanceOperationCommand command)
        {
            var handler = _walletService.BalanceOperation();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        /// <summary>
        /// Получение информации о кошельке по идентификатору.
        /// </summary>
        [HttpGet("{walletId}", Name = "GetWallet")]
        public async Task<ActionResult<WalletEntity>> GetWallet(Guid walletId)
        {
            var query = new GetWalletQuery { WalletId = walletId };
            var handler = _walletService.GetWallet();
            var wallet = await handler.Handle(query);
            return Ok(wallet);
        }

        [HttpGet("my", Name = "GetMyWallet")]
        public async Task<ActionResult<WalletEntity>> GetMyWallet()
        {
            var user = await _permService.GetCurrentUser(); 
            var query = new GetWalletQuery { UserId = user.Id };
            var handler = _walletService.GetWallet();
            var wallet = await handler.Handle(query);
            return Ok(wallet);
        }

        /// <summary>
        /// Блокировка кошелька.
        /// </summary>
        [HttpPost("block", Name = "BlockWallet")]
        public async Task<ActionResult<CommandResult>> BlockWallet([FromBody] BlockWalletCommand command)
        {
            var handler = _walletService.BlockWallet();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("payment-methods", Name = "GetPaymentMethods")]
        public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods()
        {
            var query = new GetPaymentMethodsQuery { };
            var result = await _walletService.GetPaymentMethods().Handle(query); 
            return result; 
        }

        [HttpGet("payment-methods/{userId}", Name = "GetPaymentMethodsByUser")]
        public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods(Guid userId)
        {
            var query = new GetPaymentMethodsQuery { UserId = userId};
            var result = await _walletService.GetPaymentMethods().Handle(query);
            return result;
        }

        [HttpPost("payment-methos/", Name = "AddPaymentMethod")]
        public async Task<ActionResult<CommandResult>> AddPaymentMethod([FromBody] AddPaymentMethodCommand command)
        {
            var result = await _walletService.AddPaymentMethod().Handle(command);

            return result; 
        }
    }
}
