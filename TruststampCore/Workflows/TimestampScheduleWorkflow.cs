﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Workflows;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class TimestampScheduleWorkflow : WorkflowContext
    {
        private ITimestampWorkflowService _timestampWorkflowService;
        private IConfiguration _configuration;

        public override void Execute()
        {
            _timestampWorkflowService = WorkflowService.ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
            _configuration = WorkflowService.ServiceProvider.GetRequiredService<IConfiguration>();

            if (_timestampWorkflowService.CountCurrentProofs() > 0)
            {
                _timestampWorkflowService.CreateAndExecute(); // There are proofs to be timestamp'ed
            }

            // Rerun this step after x time, never to exit
            Wait(_configuration.TimestampInterval()); // Default 10 min
        }

    }
}
