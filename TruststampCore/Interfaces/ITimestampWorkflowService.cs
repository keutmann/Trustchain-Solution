using TrustchainCore.Services;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflowService
    {
        int CurrentWorkflowID { get; }
        IWorkflowService WorkflowService { get; set; }

        void CreateNextWorkflow();
    }
}