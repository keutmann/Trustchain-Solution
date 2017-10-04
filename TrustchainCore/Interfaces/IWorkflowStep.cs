using System;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowStep : IDisposable
    {
        IWorkflowContext Context { get; set; }

        void Execute();
    }
}