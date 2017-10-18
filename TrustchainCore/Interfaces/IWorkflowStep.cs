using Newtonsoft.Json;
using System;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowStep : IDisposable
    {
        [JsonIgnore]
        IWorkflowContext Context { get; set; }

        void Execute();
    }
}