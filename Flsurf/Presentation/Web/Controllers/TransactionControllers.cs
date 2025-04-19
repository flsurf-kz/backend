using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // Обработка транзакции (например, создание или обновление транзакции)
        [HttpPost("handle", Name = "HandleTransaction")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> HandleTransaction([FromBody] HandleTransactionCommand command)
        {
            var handler = _transactionService.HandleTransaction();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        // Получение списка транзакций
        [HttpGet("list", Name = "GetTransactionsList")]
        [Authorize]
        public async Task<ActionResult<ICollection<TransactionEntity>>> GetTransactionsList(
            [FromQuery] int start = 0, [FromQuery] int end = 10)
        {
            var query = new GetTransactionsListQuery { Start = start, Ends = end };
            var handler = _transactionService.GetTransactionsList();
            var transactions = await handler.Handle(query);
            return Ok(transactions);
        }

        // Получение провайдеров транзакций
        [HttpGet("providers", Name = "GetTransactionProviders")]
        public async Task<ActionResult<ICollection<TransactionProviderEntity>>> GetTransactionProviders()
        {
            var query = new GetTransactionProvidersQuery();
            var handler = _transactionService.GetTransactionProviders();
            var providers = await handler.Handle(query);
            return Ok(providers);
        }

        // Обработка результата от шлюза депозита
        [HttpPost("gateway-webhook", Name = "HandleGatewayWebhook")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> HandleDepositGatewayResult([FromBody] GatewayResultCommand command)
        {
            var handler = _transactionService.HandleGatewayResult();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        // Возврат средств по транзакции
        [HttpPost("refund", Name = "RefundTransaction")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> RefundTransaction([FromBody] RefundTransactionCommand command)
        {
            var handler = _transactionService.RefundTransaction();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        // Запуск потока оплаты
        [HttpPost("start", Name = "StartPaymentFlow")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> StartPaymentFlow([FromBody] StartPaymentFlowCommand command)
        {
            var handler = _transactionService.StartPaymentFlow();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }
    }
}
