using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface IRemoteTimestampStep : IWorkflowStep
    {
        int RetryAttempts { get; set; }
    }
}