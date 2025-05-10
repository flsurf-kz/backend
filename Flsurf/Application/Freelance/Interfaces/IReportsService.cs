using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    // будет отвественнен за ВСЮ статистику 
    public interface IReportsService
    {
        GetFinanceSummaryHandler GetFinanceSummary();
        //GetGlobalFinancesSummaryHandler GetGlobalFinanceSummary();
        //GetJobStatisticsHandler GetJobStatistics(); 
    }
}
