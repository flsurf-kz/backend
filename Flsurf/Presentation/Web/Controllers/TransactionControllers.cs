using Flsurf.Application.Payment.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/transaction/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TransactionControllers : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;

        public TransactionControllers(ITransactionService transactionService, IWalletService walletService)
        {
            _walletService = walletService;
            _transactionService = transactionService;
        }

    }
}
