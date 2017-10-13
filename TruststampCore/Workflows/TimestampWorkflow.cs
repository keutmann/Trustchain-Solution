using System;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Workflows;
using TrustchainCore.Services;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class TimestampWorkflow : WorkflowContext
    {
        private IServiceProvider _serviceProvider;

        public TimestampWorkflow(IWorkflowService workflowService) : base(workflowService)
        {
            _serviceProvider = workflowService.ServiceProvider;
        }

        public override void Initialize()
        {
            base.Initialize();
            Steps.Add(_serviceProvider.GetRequiredService<IMerkleStep>());

            foreach (var step in Steps)
            {
                step.Context = this;
            }
        }
    }
}
