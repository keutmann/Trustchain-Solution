using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustchainCore.Services;

namespace TrustchainCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Trustchain(this IApplicationBuilder app, IServiceCollection services)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            { 
                var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                workflowService.RunWorkflows(services);
            }

        }
    }
}
