using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/portfolio-project")]
    public class ReportsService(IReportsService reportsService) : ControllerBase
    {
        [HttpPost("/finances-summary", Name = "GetUserFinancesSummary")]
        [Authorize]
        public async Task<ActionResult<FinanceSummaryDto>> GetFinancesSummary([FromBody] GetFinanceSummaryQuery query)
        {
            return await reportsService.GetFinanceSummary().Handle(query); 
        }
    }
}
