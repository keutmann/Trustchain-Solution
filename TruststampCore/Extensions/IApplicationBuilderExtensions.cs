using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Truststamp(this IApplicationBuilder app)
        {
            //var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            var timestampWorkflowService = app.ApplicationServices.GetRequiredService<ITimestampWorkflowService>();
            timestampWorkflowService.RunWorkflows();
        }
    }
}
