using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Workflows
{
    /// <summary>
    /// Makes sure to timestamp a package
    /// </summary>
    public class TrustTimestampWorkflow : WorkflowContext, ITrustTimestampWorkflow
    {
        private IServiceProvider _serviceProvider;

        public TrustTimestampWorkflow(IWorkflowService workflowService, IServiceProvider serviceProvider) : base(workflowService)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Initialize()
        {
            Steps.Add(_serviceProvider.GetRequiredService<ITrustTimestampStep>());
            base.Initialize();
        }

        public override Task Execute()
        {
            return base.Execute();
        }
    }

}
