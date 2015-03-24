using Hangfire.Dashboard;
using HangfireMvc;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace HangfireMvc
{
    public class Startup 
    {
        public void Configuration(IAppBuilder app)
        {
            var authorizationFilters = new[]
            {
                new LocalRequestsOnlyAuthorizationFilter()
            };

            app.MapHangfireDashboard("/hangfire", authorizationFilters);       
        }

    }
}
