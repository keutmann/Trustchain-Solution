using System;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Workflows
{
    public class WorkflowLog : IWorkflowLog
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public WorkflowLog()
        {
            Time = DateTime.Now;
        }
    }
}
