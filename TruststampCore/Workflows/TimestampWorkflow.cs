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
        private ITimestampSynchronizationService _timestampSynchronizationService;

        public TimestampWorkflow(IWorkflowService workflowService) : base(workflowService)
        {
            _serviceProvider = workflowService.ServiceProvider;
            _timestampSynchronizationService = _serviceProvider.GetRequiredService<ITimestampSynchronizationService>();
        }

        public override void Initialize()
        {
            Steps.Add(_serviceProvider.GetRequiredService<IMerkleStep>());
            base.Initialize();
        }

        public override bool DoExecution()
        {
            if (!base.DoExecution())
                return false;

            if (ID == _timestampSynchronizationService.CurrentWorkflowID)
                return false; // Do not execute this workflow, because Proofs are still beeing added to it.

            return true;
        }
    }
}
