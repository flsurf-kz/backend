using Hangfire;

namespace Flsurf.Infrastructure.BackgroundJobs
{
    public static class BackgroundJobsRegister
    {
        public static void RegisterInfrastructureBGJobs()
        {
            RecurringJob.AddOrUpdate<SessionCleanupJob>(
                "ProcessJobHourly",              // Идентификатор задания
                service => service.Execute(), // Метод для выполнения
                Cron.Hourly                      // Расписание: каждый час
            );
        }
    }
}
