using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustchainCore.Services
{
    public interface IWorkflowService : IDisposable
    {
        //ITrustDBService DBService { get; set; }
        [JsonIgnore]
        IServiceProvider ServiceProvider { get; set; }
        IQueryable<WorkflowContainer> Workflows { get;  }

        T Load<T>(int id) where T : class, IWorkflowContext;
        IWorkflowContext Create(WorkflowContainer container);
        T Create<T>() where T : class, IWorkflowContext;
        //void Execute(Dictionary<string, bool> workflows, WorkflowContainer container, );
        int Save(IWorkflowContext workflow);
        //WorkflowContainer Load();
        WorkflowContainer CreateWorkflowContainer(IWorkflowContext workflow);
        IList<WorkflowContainer> GetRunningWorkflows();
        void RunWorkflows(IServiceCollection services);
        void Execute(ConcurrentDictionary<string, bool> workflows, WorkflowContainer container, IServiceCollection services);
        void ExecuteAsync(ConcurrentDictionary<string, bool> workflows, WorkflowContainer container, IServiceCollection services);
        //T GetRunningWorkflow<T>() where T : class, IWorkflowContext;
        T EnsureWorkflow<T>() where T : class, IWorkflowContext;
    }
}