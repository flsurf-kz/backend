using Hangfire.Dashboard;

namespace Flsurf.Presentation.Web.Services
{
    public class CookieDashboardAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
            => context.GetHttpContext().User.IsInRole("Admin"); 
    }
}
