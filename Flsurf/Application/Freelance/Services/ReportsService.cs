using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class ReportsService(IServiceProvider serviceProvider) : IReportsService
    {
        public GetFinanceSummaryHandler GetFinanceSummary()
        {
            return serviceProvider.GetRequiredService<GetFinanceSummaryHandler>();
        }
    }
}
