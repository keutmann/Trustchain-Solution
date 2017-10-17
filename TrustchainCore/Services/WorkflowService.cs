using System;
using System.Linq;
using Newtonsoft.Json;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Enumerations;
using Newtonsoft.Json.Serialization;
using TrustchainCore.Extensions;
using TrustchainCore.Workflows;

namespace TrustchainCore.Services
{
    public class WorkflowService : IWorkflowService
    {
        private ITrustDBService _trustDBService;
        private IExecutionSynchronizationService _executionSynchronizationService;
        public IServiceProvider ServiceProvider { get; set; }

        public IQueryable<WorkflowContainer> Workflows {
            get
            {
                return _trustDBService.Workflows;
            }
        }

        public WorkflowService(ITrustDBService trustDBService, IServiceProvider serviceProvider, IExecutionSynchronizationService executionSynchronizationService)
        {
            _trustDBService = trustDBService;
            _executionSynchronizationService = executionSynchronizationService;
            ServiceProvider = serviceProvider;
        }

        public IWorkflowContext Create(WorkflowContainer container) 
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = ServiceProvider.GetService<IContractResolver>(),
                TypeNameHandling = TypeNameHandling.Auto
            };
            settings.Converters.Add(new DICustomConverter<WorkflowContext>(ServiceProvider));


            var instance = JsonConvert.DeserializeObject<WorkflowContext>(container.Data, settings);
            foreach (var step in instance.Steps)
            {
                step.Context = instance;
            }

            instance.ID = container.ID;
            instance.State = container.State;
            instance.Tag = container.Tag;

            return instance;
        }

        public T Create<T>(WorkflowContainer container = null) where T : class, IWorkflowContext
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = ServiceProvider.GetService<IContractResolver>(),
                TypeNameHandling = TypeNameHandling.Auto
            };
            settings.Converters.Add(new DICustomConverter<T>(ServiceProvider));


            T instance = default(T);
            if(container == null || String.IsNullOrWhiteSpace(container.Data))
            {
                instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
                //instance = JsonConvert.DeserializeObject<T>("{}", settings);
                instance.State = WorkflowStatusType.New.ToString();
                instance.Initialize(); // Initialize new workflow
                
            }
            else
            {
                instance = JsonConvert.DeserializeObject<T>(container.Data, settings);
                foreach (var step in instance.Steps)
                {
                    step.Context = instance;
                } 
            }

            if (container != null)
            {
                instance.ID = container.ID;
                instance.State = container.State;
                instance.Tag = container.Tag;
            }

            return instance;
        }

        public void Execute(IList<IWorkflowContext> workflows)
        {
            var executing = new List<Task>();
            foreach (var workflow in workflows)
            {
                if (_executionSynchronizationService.Workflows.ContainsKey(workflow.ID))
                    continue; // Ignore the workflow, because its allready running!

                _executionSynchronizationService.Workflows.TryAdd(workflow.ID, workflow);
                var task = workflow.Execute();
                task.ContinueWith(t => _executionSynchronizationService.Workflows.TryRemove(workflow.ID, out IWorkflowContext value));

                executing.Add(task);
            }

            Task.WaitAll(executing.ToArray());
        }

        public int Save(IWorkflowContext workflow)
        {
            if(workflow.ID != 0)
            {
                var entity = _trustDBService.Workflows.FirstOrDefault(w => w.ID == workflow.ID);
                if (entity != null)
                {
                    entity.Type = workflow.GetType().FullName;
                    entity.State = workflow.State;
                    entity.Tag = workflow.Tag;
                    entity.Data = JsonConvert.SerializeObject(workflow);
                    _trustDBService.DBContext.SaveChanges();
                    return workflow.ID; // Exit now!
                }
            }

            var container = CreateWorkflowContainer(workflow);
            _trustDBService.DBContext.Workflows.Add(container);
            _trustDBService.DBContext.SaveChanges();
            workflow.ID = container.ID;
            return container.ID;
        }

        public WorkflowContainer CreateWorkflowContainer(IWorkflowContext workflow)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = ServiceProvider.GetService<IContractResolver>(),
                TypeNameHandling = TypeNameHandling.Auto
            };

            var entity = new WorkflowContainer
            {
                Type = workflow.GetType().FullName,
                State = workflow.State,
                Tag = workflow.Tag,
                Data = JsonConvert.SerializeObject(workflow, settings)
            };
            return entity;
        }

        public IList<IWorkflowContext> GetRunningWorkflows()
        {
            var list = new List<IWorkflowContext>();
            var containers = from p in _trustDBService.Workflows
                             where (p.State == WorkflowStatusType.New.ToString() || p.State == WorkflowStatusType.Running.ToString())
                             select p;

            foreach (var container in containers)
            {
                list.Add(Create(container));
            }
            return list;
        }

        public void RunWorkflows()
        {
            int id = 0;
            var taskProcessor = new System.Timers.Timer { Interval = 1000 }; // Run the interval 1 sec
            taskProcessor.Elapsed += (sender, e) =>
            {
                var localID = id++;
                var scopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var workflows = GetRunningWorkflows();
                    Execute(workflows);
                }
            };
            taskProcessor.Start();
        }

    }
}
