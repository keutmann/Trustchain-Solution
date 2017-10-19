using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface ILocalTimestampStep : IWorkflowStep
    {
        int RetryAttempts { get; set; }
    }
}