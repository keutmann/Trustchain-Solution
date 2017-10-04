using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Enumerations;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowContext
    {
        int ID { get; set; }
        List<IWorkflowLog> Logs { get; set; }
        string State { get; set; }
        WorkflowStatus Status { get; set; }
        IList<IWorkflowStep> Steps { get; set; }
        string Tag { get; set; }

        void Initialize();
        Task Execute();

    }
}