using TrustgraphCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using System;

namespace TrustgraphCore.Workflows
{
    /// <summary>
    /// Makes sure to timestamp a package
    /// </summary>
    public class TrustPackageWorkflow : WorkflowContext, ITrustPackageWorkflow
    {
        private IServiceProvider _serviceProvider;

        public TrustPackageWorkflow(IWorkflowService workflowService, IServiceProvider serviceProvider) : base(workflowService)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Initialize()
        {
            //Steps.Add(_serviceProvider.GetRequiredService<ITimestampScheduleStep>());
            base.Initialize();
        }

        public override void Execute()
        {
            base.Execute();
        }
    }

}
