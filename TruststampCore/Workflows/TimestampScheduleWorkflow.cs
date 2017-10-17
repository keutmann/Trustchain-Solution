using Microsoft.Extensions.DependencyInjection;
using System;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using TruststampCore.Services;

namespace TruststampCore.Workflows
{
    public class TimestampScheduleWorkflow : WorkflowContext
    {
        private IServiceProvider _serviceProvider;

        public TimestampScheduleWorkflow(IWorkflowService workflowService) : base(workflowService)
        {
            _serviceProvider = workflowService.ServiceProvider;
        }

        public override void Initialize()
        {
            Steps.Add(_serviceProvider.GetRequiredService<IRunTimestampStep>());
            base.Initialize();
        }

    }
}
