using System.Collections.Concurrent;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Services
{
    public class ExecutionSynchronizationService : IExecutionSynchronizationService
    {
        public ConcurrentDictionary<int, IWorkflowContext> Workflows { get; set; }

        public ExecutionSynchronizationService()
        {
            Workflows = new ConcurrentDictionary<int, IWorkflowContext>();
        }
    }
}
