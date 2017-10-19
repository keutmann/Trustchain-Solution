using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface IAddressVerifyStep : IWorkflowStep
    {
        int RetryAttempts { get; set; }
    }
}