using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustchainCore.Services;

namespace TrustchainCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Trustchain(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                workflowService.RunWorkflows();
            }

        }
    }
}
