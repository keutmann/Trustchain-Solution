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
using System.Collections.Concurrent;

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
            _trustDBService.ID = DateTime.Now.Ticks;

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
            if (container == null)
                return default(T);
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
            instance.Container = container;
            instance.WorkflowService = this;
            foreach (var step in instance.Steps)
            {
                step.Context = instance;
            }

            instance.ID = container.DatabaseID;

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
            var settings = new JsonSerializerSettings
            {
                ContractResolver = _contractReverseResolver,
                TypeNameHandling = TypeNameHandling.Objects
            };
            workflow.Container.Data = JsonConvert.SerializeObject(workflow, settings);

            if (workflow.Container.DatabaseID != 0)
            {
                //_trustDBService.DBContext.Workflows.Update(workflow.Container);
                var result = _trustDBService.DBContext.SaveChanges();
            }
            else
            {
                _trustDBService.DBContext.Workflows.Add(workflow.Container);
                _trustDBService.DBContext.SaveChanges();
                workflow.ID = workflow.Container.DatabaseID;
            }

            return workflow.ID; // Exit now!
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
                State = workflow.Container.State,
                Tag = workflow.Container.Tag,
                Data = JsonConvert.SerializeObject(workflow, settings)
            };
            return entity;
        }

        public async void RunWorkflows(IServiceCollection services)
        {
            await Task.Run(() => {
                _logger.DateInformation($"TaskProcessor started");

                var runningWorkflows = new ConcurrentDictionary<string, bool>();

                while (true)
                {
                    
                    using (var localScope = this.ServiceProvider.CreateScope())
                    //using(var localScope = services.BuildServiceProvider().CreateScope())
                    {
                        var localWorkflowService = localScope.ServiceProvider.GetRequiredService<IWorkflowService>();
                        var containers = localWorkflowService.GetRunningWorkflows();

                        if (containers.Count > 0)
                        {
                            _logger.DateInformation($"Active Workflows found: {containers.Count}");

                            foreach (var container in containers)
                            {
                                // Run workflows serialized! May still be problems with the async parallel processing
                                localWorkflowService.Execute(runningWorkflows, container, services);
                            }

                            _logger.DateInformation($"TaskProcessor done");
                        }

                        _trustDBService.DBContext.Dispose();
                    }

                    GC.Collect();
                    Task.Delay(_configuration.WorkflowInterval()).Wait();
                }
            });
        }

        public IList<WorkflowContainer> GetRunningWorkflows()
        {
            var time = DateTime.Now.ToUnixTime();
            //var list = new List<IWorkflowContext>();
            var containers = (from p in _trustDBService.Workflows
                             where (p.State == WorkflowStatusType.New.ToString()
                                 || p.State == WorkflowStatusType.Starting.ToString()
                                 || p.State == WorkflowStatusType.Running.ToString()
                                 || p.State == WorkflowStatusType.Waiting.ToString())
                                 && p.NextExecution <= time
                              select p).ToArray();
            return containers;
        }

        public async void ExecuteAsync(ConcurrentDictionary<string, bool> workflows, WorkflowContainer container, IServiceCollection services)
        {
            if (workflows.ContainsKey(container.Type))
                return;

            if (!workflows.TryAdd(container.Type, true))
                return;

            _logger.DateInformation($"Executing workflow id : {container.DatabaseID}");

            await Task.Run(() => {
                // Make a scope for the workflow to run in.
                //var taskServiceProvider = services.BuildServiceProvider();
                using (var taskScope = ServiceProvider.CreateScope())
                {
                    var taskWorkflowService = taskScope.ServiceProvider.GetRequiredService<IWorkflowService>();
                    var workflow = taskWorkflowService.Create(container);

                    workflow.Execute();
                }
            });

            workflows.TryRemove(container.Type, out bool val);

            _logger.DateInformation($"ContinueWith -> Done executing workflow id {container.DatabaseID}");
        }

        public void Execute(ConcurrentDictionary<string, bool> workflows, WorkflowContainer container, IServiceCollection services)
        {
            if (workflows.ContainsKey(container.Type))
                return;

            if (!workflows.TryAdd(container.Type, true))
                return;

            _logger.DateInformation($"Executing workflow id : {container.DatabaseID}");

            var workflow = Create(container);
            workflow.Execute();

            workflows.TryRemove(container.Type, out bool val);

            _logger.DateInformation($"ContinueWith -> Done executing workflow id {container.DatabaseID}");
        }


        public T EnsureWorkflow<T>() where T : class, IWorkflowContext
        {
            var container = Workflows.FirstOrDefault(p => p.Type == typeof(T).FullName
                                 && (p.State == WorkflowStatusType.New.ToString()
                                 || p.State == WorkflowStatusType.Starting.ToString()
                                 || p.State == WorkflowStatusType.Running.ToString()
                                 || p.State == WorkflowStatusType.Waiting.ToString()));

            if (container == null)
            {
                var wf = Create<T>();
                Save(wf);
                return wf;
            }

            var runningWf = (T)Create(container);
            return runningWf;
        }
    }
}

