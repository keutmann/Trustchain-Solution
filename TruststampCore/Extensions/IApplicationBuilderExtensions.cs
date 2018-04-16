using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Truststamp(this IApplicationBuilder app)
        {
            // Ensure that a Timestamp workflow is running.
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var timestampWorkflowService = scope.ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
                //var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                //var trustDBService = scope.ServiceProvider.GetRequiredService<ITrustDBService>();

                //// Do some clean up, because 
                //var workflows = trustDBService.Workflows.Select(p => p);
                //trustDBService.DBContext.Workflows.RemoveRange(workflows.ToArray());
                //trustDBService.DBContext.SaveChanges();

                //TODO: Undelete
                timestampWorkflowService.CreateAndExecute(); // Make sure that there is a Timestamp engine workflow
            }
        }
    }
}
