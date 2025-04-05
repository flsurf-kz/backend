using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;
using Flsurf.Domain.Payment.Entities;
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

        public WalletController(IWalletService walletService)
        {
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
            if (wallet == null)
                return NotFound("Кошелёк не найден");
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
    }
}
