using System;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowLog
    {
        string Message { get; set; }
        DateTime Time { get; set; }
    }
}