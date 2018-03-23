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
using Newtonsoft.Json.Linq;

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
            //instance.WorkflowService = this;
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

        public Task RunWorkflows()
        {
            return Task.Run(() => {

                while (true)
                {
                    int id = 0;
                    var localID = id++;

                    var scopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = scopeFactory.CreateScope())
                    {
                        Task.Run(() =>
                        {
                            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WorkflowService>>();

                            logger.DateInformation(localID, $"TaskProcessor started");

                            var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();

                            var workflows = workflowService.GetRunningWorkflows();
                            logger.DateInformation(localID, $"Workflows found: {workflows.Count}");

                            workflowService.Execute(workflows, localID);

                            logger.DateInformation(localID, $"TaskProcessor done");
                        }).Wait();
                    }

                    Task.Delay(_configuration.WorkflowInterval()).Wait();
                }
            });
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

        public void Execute(IList<IWorkflowContext> workflows, int localID = 0)
        {
            var executing = new List<Task>();
            foreach (var workflow in workflows)
            {
                _logger.DateInformation(localID, $"Executing workflow id : {workflow.ID}");
                //var task = workflow.Execute().ContinueWith(t => {
                //    _logger.DateInformation(localID, $"ContinueWith -> Done executing workflow id {workflow.ID}");
                //});

                workflow.Execute().Wait();
                _logger.DateInformation(localID, $"ContinueWith -> Done executing workflow id {workflow.ID}");
                //executing.Add(task);
            }

            //Task.WaitAll(executing.ToArray());
        }
    }
}

