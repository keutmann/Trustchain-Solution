using TrustchainCore.Services;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Workflows;

namespace TrustgraphCore.Services
{
    public class GraphWorkflowService : IGraphWorkflowService
    {
        private IWorkflowService _workflowService;

        public GraphWorkflowService(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }


        public void EnsureTrustTimestampWorkflow()
        {
            _workflowService.EnsureWorkflow<TrustTimestampWorkflow>();
        }

    }
}
