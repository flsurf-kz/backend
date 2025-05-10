using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Services;

namespace Flsurf.Application.Freelance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFreelancerApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IContestService, ContestService>(); 
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IFreelancerProfileService, FreelancerProfileService>();
            services.AddScoped<IFreelancerTeamService, FreelancerTeamService>();
            services.AddScoped<IJobService, JobService>(); 
            services.AddScoped<IPortfolioProjectService, PortfolioProjectService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IClientProfileService, ClientProfileService>(); 
            services.AddScoped<IWorkSessionService, WorkSessionService>();
            services.AddScoped<IReportsService, ReportsService>(); 

            return services;
        }
    }
}
