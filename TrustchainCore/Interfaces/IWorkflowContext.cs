using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Services;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowContext 
    {
        [JsonIgnore]
        IWorkflowService WorkflowService { get; set; }

        int ID { get; set; }
        List<IWorkflowLog> Logs { get; set; }
        string State { get; set; }
        IList<IWorkflowStep> Steps { get; set; }
        int CurrentStepIndex { get; set; }
        string Tag { get; set; }
        DateTime NextExecution { get; set; }

        bool DoExecution();
        void RunStepAgain(int seconds);
        void Wait(int seconds);

        void Initialize();
        Task Execute();
        T GetStep<T>();
        void Log(string message);
    }
}