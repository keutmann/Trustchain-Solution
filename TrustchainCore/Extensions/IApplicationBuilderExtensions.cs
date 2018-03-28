using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustchainCore.Services;

namespace TrustchainCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Trustchain(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            //var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            //var scope = scopeFactory.CreateScope();
            //using (var scope = scopeFactory.CreateScope())
            //{
            var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
            workflowService.RunWorkflows();
            //}

        }
    }
}
