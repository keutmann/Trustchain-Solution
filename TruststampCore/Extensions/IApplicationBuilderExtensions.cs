using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Truststamp(this IApplicationBuilder app)
        {
            // Ensure that a Timestamp workflow is running.

            var timestampWorkflowService = app.ApplicationServices.GetRequiredService<ITimestampWorkflowService>();
            timestampWorkflowService.EnsureTimestampWorkflow();

    }
}
}
