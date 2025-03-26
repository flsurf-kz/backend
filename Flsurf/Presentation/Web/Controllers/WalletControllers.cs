using Flsurf.Application.Payment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [Route("api/wallet/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class WalletControllers : ControllerBase
    {
        private IWalletService _walletService;

        public WalletControllers(IWalletService walletService)
        {
            _walletService = walletService;
        }
    }
}
