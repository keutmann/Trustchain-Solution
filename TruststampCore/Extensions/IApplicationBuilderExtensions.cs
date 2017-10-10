using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Timestamp(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var timestampWorkflowService = scope.ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
                timestampWorkflowService.CreateNextWorkflow();
            }
        }
    }
}
