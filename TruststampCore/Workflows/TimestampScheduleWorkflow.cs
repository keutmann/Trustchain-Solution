using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Services;
using TrustchainCore.Workflows;

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
            base.Initialize();
            //Steps.Add(_serviceProvider.GetRequiredService<IMerkleStep>());

            foreach (var step in Steps)
            {
                step.Context = this;
            }
        }

    }
}
