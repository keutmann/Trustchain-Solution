using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustchainCore.Services
{
    public interface IWorkflowService
    {
        //ITrustDBService DBService { get; set; }
        [JsonIgnore]
        IServiceProvider ServiceProvider { get; set; }
        IQueryable<WorkflowContainer> Workflows { get;  }

        IWorkflowContext Create(WorkflowContainer container);
        T Create<T>() where T : class, IWorkflowContext;
        void Execute(IList<IWorkflowContext> workflows, int localID = 0);
        int Save(IWorkflowContext workflow);
        //WorkflowContainer Load();
        WorkflowContainer CreateWorkflowContainer(IWorkflowContext workflow);
        IList<IWorkflowContext> GetRunningWorkflows();
        void RunWorkflows();
    }
}