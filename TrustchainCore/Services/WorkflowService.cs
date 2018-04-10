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
        private IContractResolver _contractResolver;
        private IContractReverseResolver _contractReverseResolver;
        private ILogger _logger;
        private IConfiguration _configuration;

        public IQueryable<WorkflowContainer> Workflows {
            get
            {
                return _trustDBService.Workflows;
            }
        }

        public WorkflowService(ITrustDBService trustDBService, IContractResolver contractResolver, IContractReverseResolver contractReverseResolver, 
            IExecutionSynchronizationService executionSynchronizationService, ILogger<WorkflowService> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _trustDBService = trustDBService;
            _executionSynchronizationService = executionSynchronizationService;
            _contractResolver = contractResolver;
            _contractReverseResolver = contractReverseResolver;
            _logger = logger;
            _configuration = configuration;
            ServiceProvider = serviceProvider;
        }

        public T Load<T>(int id) where T : class, IWorkflowContext
        {
            var container = Workflows.FirstOrDefault(p => p.DatabaseID == id);
            var workflow = (T)Create(container);
            return workflow;
        }

        public IWorkflowContext Create(WorkflowContainer container) 
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                TypeNameHandling = TypeNameHandling.Objects
            };

            var instance = (WorkflowContext)JsonConvert.DeserializeObject(container.Data, settings);
            instance.WorkflowService = this;
            foreach (var step in instance.Steps)
            {
                step.Context = instance;
            }

            instance.ID = container.DatabaseID;
            instance.State = container.State;
            instance.Tag = container.Tag;

            return instance;
        }

        public T Create<T>() where T : class, IWorkflowContext
        {

            //T instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
            //var settings = new JsonSerializerSettings
            //{
            //    ContractResolver = _contractResolver,
            //    TypeNameHandling = TypeNameHandling.Objects
            //};

            //var instance = JsonConvert.DeserializeObject<T>("{}", settings);
            var instance = ServiceProvider.GetRequiredService<T>();

            //T instance =  (T)Activator.CreateInstance(typeof(T), new object[] { this });
            instance.Initialize(); // Initialize new workflow
                
            return instance;
        }



        public int Save(IWorkflowContext workflow)
        {
            if(workflow.ID != 0)
            {
                var entity = _trustDBService.Workflows.FirstOrDefault(w => w.DatabaseID == workflow.ID);
                if (entity != null)
                {
                    entity.Type = workflow.GetType().FullName;
                    entity.State = workflow.State;
                    entity.Tag = workflow.Tag;

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = _contractReverseResolver,
                        TypeNameHandling = TypeNameHandling.Objects
                    };

                    entity.Data = JsonConvert.SerializeObject(workflow, settings);

                    _trustDBService.DBContext.SaveChanges();

                    return workflow.ID; // Exit now!
                }
            }

            var container = CreateWorkflowContainer(workflow);
            _trustDBService.DBContext.Workflows.Add(container);
            _trustDBService.DBContext.SaveChanges();
            workflow.ID = container.DatabaseID;
            return container.DatabaseID;
        }

        public WorkflowContainer CreateWorkflowContainer(IWorkflowContext workflow)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = _contractReverseResolver,
                TypeNameHandling = TypeNameHandling.Objects
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

        public async void RunWorkflows(IServiceCollection services)
        {
            await Task.Run(() => {
                _logger.DateInformation($"TaskProcessor started");

                var runningWorkflows = new Dictionary<string, bool>();

                while (true)
                {
                    using (var localScope = this.ServiceProvider.CreateScope())
                    {
                        var localWorkflowService = localScope.ServiceProvider.GetRequiredService<IWorkflowService>();
                        var containers = localWorkflowService.GetRunningWorkflows();

                        _logger.DateInformation($"Workflows found: {containers.Count}");

                        foreach (var container in containers)
                        {
                            ExecuteAsync(runningWorkflows, container, services);

                            //_logger.DateInformation($"Executing workflow id : {container.DatabaseID}");

                            //workflow.Execute();

                            //_logger.DateInformation($"ContinueWith -> Done executing workflow id {container.DatabaseID}");
                        }

                        _logger.DateInformation($"TaskProcessor done");
                    }

                    Task.Delay(_configuration.WorkflowInterval()).Wait();
                }
            });
        }

        public IList<WorkflowContainer> GetRunningWorkflows()
        {
            //var list = new List<IWorkflowContext>();
            var containers = (from p in _trustDBService.Workflows
                             where (p.State == WorkflowStatusType.New.ToString() || p.State == WorkflowStatusType.Running.ToString())
                             select p).ToArray();
            return containers;
        }

        public async void ExecuteAsync(Dictionary<string, bool> workflows, WorkflowContainer container, IServiceCollection services)
        {
            if (workflows.ContainsKey(container.Type))
                return;

            _logger.DateInformation($"Executing workflow id : {container.DatabaseID}");

            workflows.Add(container.Type, true);

            await Task.Run(() => {
                // Make a scope for the workflow to run in.
                //var taskServiceProvider = services.BuildServiceProvider();
                using (var taskScope = ServiceProvider.CreateScope())
                {
                    var taskWorkflowService = taskScope.ServiceProvider.GetRequiredService<IWorkflowService>();
                    var workflow = taskWorkflowService.Create(container);

                    if (workflow.DoExecution())
                        workflow.Execute();
                }
            });

            workflows.Remove(container.Type);

            _logger.DateInformation($"ContinueWith -> Done executing workflow id {container.DatabaseID}");
        }


        public void EnsureWorkflow<T>() where T : class, IWorkflowContext
        {
            var workflowContainer = Workflows.FirstOrDefault(p => p.Type == typeof(T).FullName
                                             && (p.State == WorkflowStatusType.New.ToString()
                                             || p.State == WorkflowStatusType.Running.ToString()));

            if (workflowContainer == null)
            {
                var wf = Create<T>();
                Save(wf);
            }
        }
    }
}

