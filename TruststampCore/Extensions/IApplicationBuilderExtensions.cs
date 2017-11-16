﻿using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;
using TrustchainCore.Interfaces;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Truststamp(this IApplicationBuilder app)
        {
            // Ensure that a Timestamp workflow is running.
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var timestampWorkflowService = scope.ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
                var trustDBService = scope.ServiceProvider.GetRequiredService<ITrustDBService>();

                // Do some clean up, because 
                var workflows = trustDBService.Workflows.Select(p => p);
                trustDBService.DBContext.Workflows.RemoveRange(workflows.ToArray());
                trustDBService.DBContext.SaveChanges();

                timestampWorkflowService.EnsureTimestampWorkflow(); // Make sure that there is a Timestamp engine workflow
                timestampWorkflowService.CreateNextTimestampWorkflow(); // Make sure that the CurrentWorkflowID is set for Proof handling
            }
        }
    }
}
