﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TruststampCore.Interfaces;

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

                timestampWorkflowService.EnsureTimestampScheduleWorkflow();
                timestampWorkflowService.CreateAndExecute(); // Make sure that there is a Timestamp engine workflow
            }
        }
    }
}
