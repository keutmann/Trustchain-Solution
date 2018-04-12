using System.Collections.Generic;
using TrustchainCore.Services;
using TruststampCore.Workflows;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflowService
    {
        IWorkflowService WorkflowService { get; }
        int CountCurrentProofs();
        void CreateAndExecute();
        void EnsureTimestampScheduleWorkflow();
        void EnsureTimestampWorkflow();
    }
}