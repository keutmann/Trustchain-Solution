using System.Collections.Generic;
using TrustchainCore.Services;
using TruststampCore.Workflows;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflowService
    {
        int CountCurrentProofs();
        void InitiliazeTimestampSynchronizationService();
        void EnsureTimestampScheduleWorkflow();
        void EnsureTimestampWorkflow();
    }
}