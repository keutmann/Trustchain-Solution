using System.Collections.Concurrent;

namespace TrustchainCore.Interfaces
{
    public interface IExecutionSynchronizationService
    {
        ConcurrentDictionary<int, IWorkflowContext> Workflows { get; set; }
    }
}
