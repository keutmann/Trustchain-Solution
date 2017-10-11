using TrustchainCore.Services;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflowService
    {
        int CurrentWorkflowID { get; }
        void CreateNextWorkflow();
    }
}