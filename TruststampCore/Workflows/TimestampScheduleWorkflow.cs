using Microsoft.Extensions.DependencyInjection;
using System;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using TruststampCore.Interfaces;
using TruststampCore.Services;

namespace TruststampCore.Workflows
{
    public class TimestampScheduleWorkflow : WorkflowContext
    {
        private IServiceProvider _serviceProvider;

        public TimestampScheduleWorkflow(IWorkflowService workflowService, IServiceProvider serviceProvider) : base(workflowService)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Initialize()
        {
            Steps.Add(_serviceProvider.GetRequiredService<ITimestampScheduleStep>());
            base.Initialize();
        }

    }
}
