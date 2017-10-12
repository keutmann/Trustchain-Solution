using System;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Workflows;
using TrustchainCore.Services;

namespace TruststampCore.Workflows
{
    public class TimestampWorkflow : WorkflowContext
    {
        private IServiceProvider _serviceProvider;

        public TimestampWorkflow(IWorkflowService workflowService, IServiceProvider serviceProvider) : base(workflowService)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Initialize()
        {
            Steps.Add(_serviceProvider.GetRequiredService<MerkleStep>());
        }
    }
}
