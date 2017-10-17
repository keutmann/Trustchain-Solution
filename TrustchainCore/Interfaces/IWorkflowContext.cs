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
        WorkflowStatusType Status { get; set; }
        IList<IWorkflowStep> Steps { get; set; }
        string Tag { get; set; }

        bool DoExecution();
        void RunStepAgain(int seconds);
        void Wait(int seconds);

        void Initialize();
        Task Execute();
        T GetStep<T>();
        void Log(string message);
    }
}