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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrustchainCore.Services
{
    public class WorkflowService : IWorkflowService
    {
        private ITrustDBService _trustDBService;
        private IExecutionSynchronizationService _executionSynchronizationService;
        public IServiceProvider ServiceProvider { get; set; }
        private ILogger _logger;

        public IQueryable<WorkflowContainer> Workflows {
            get
            {
                return _trustDBService.Workflows;
            }
        }

        public WorkflowService(ITrustDBService trustDBService, IServiceProvider serviceProvider, IExecutionSynchronizationService executionSynchronizationService, ILogger<WorkflowService> logger)
        {
            _trustDBService = trustDBService;
            _executionSynchronizationService = executionSynchronizationService;
            ServiceProvider = serviceProvider;
            _logger = logger;
        }

        public IWorkflowContext Create(WorkflowContainer container) 
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = ServiceProvider.GetService<IContractResolver>(),
                TypeNameHandling = TypeNameHandling.Objects | TypeNameHandling.Arrays
            };
            settings.Converters.Add(new DICustomConverter<IWorkflowContext>(ServiceProvider));


            var instance = (WorkflowContext)JsonConvert.DeserializeObject(container.Data, settings);
            foreach (var step in instance.Steps)
            {
                step.Context = instance;
            }

            instance.ID = container.ID;
            instance.State = container.State;
            instance.Tag = container.Tag;

            return instance;
        }

        public T Create<T>() where T : class, IWorkflowContext
        {
            T instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
            instance.Initialize(); // Initialize new workflow
                
            return instance;
        }

        public void Execute(IList<IWorkflowContext> workflows, int localID = 0)
        {
            var executing = new List<Task>();
            foreach (var workflow in workflows)
            {
                _logger.LogInformation(localID, $"ExecutionSynchronizationService contains {_executionSynchronizationService.Workflows.Count} workflows");
                if (_executionSynchronizationService.Workflows.ContainsKey(workflow.ID))
                {
                    _logger.LogInformation(localID, $"ExecutionSynchronizationService contains a workflow with id : {workflow.ID}");
                    continue; // Ignore the workflow, because its allready running!
                }

                _executionSynchronizationService.Workflows.TryAdd(workflow.ID, workflow);
                _logger.LogInformation(localID, $"Executing workflow id : {workflow.ID}");
                var task = workflow.Execute().ContinueWith(t => {
                    _executionSynchronizationService.Workflows.TryRemove(workflow.ID, out IWorkflowContext value);
                    _logger.LogInformation(localID, $"ContinueWith -> Done executing workflow id {workflow.ID}");
                    });
                
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
                TypeNameHandling = TypeNameHandling.Objects | TypeNameHandling.Arrays
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
            var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            int id = 0;
            var taskProcessor = new System.Timers.Timer { Interval = configuration.GetValue<int>("WorkflowInterval", 1000) }; // Run the interval 1 sec
            taskProcessor.Elapsed += (sender, e) =>
            {
                taskProcessor.Enabled = false;

                var localID = id++;
                var scopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    _logger.LogInformation(localID, $"GetRunningWorkflows");
                    var workflows = GetRunningWorkflows();
                    _logger.LogInformation(localID, $"Workflows found: {workflows.Count}");
                    Execute(workflows, localID);
                    _logger.LogInformation(localID, $"Done executing Workflows");
                }
                taskProcessor.Enabled = true;
            };
            taskProcessor.Start();
        }

    }
}
