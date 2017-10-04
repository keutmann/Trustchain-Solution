using System;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Workflows;
using TrustchainCore.Services;

namespace TruststampCore.Workflows
{
    public class TimestampWorkflow : WorkflowContext
    {

        public TimestampWorkflow(IWorkflowService workflowService) : base(workflowService)
        {
        }

        public override void Initialize()
        {
            Steps.Add(this._workflowService.ServiceProvider.GetRequiredService<MerkleStep>());
        }
    }
}
