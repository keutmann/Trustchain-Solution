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

namespace TrustchainCore.Services
{
    public class WorkflowService : IWorkflowService
    {
        private ITrustDBService _trustDBService;
        public IServiceProvider ServiceProvider { get; set; }

        public WorkflowService(ITrustDBService trustDBService, IServiceProvider serviceProvider)
        {
            _trustDBService = trustDBService;
            ServiceProvider = serviceProvider;
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
            foreach (var context in workflows)
            {
                executing.Add(context.Execute());
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

    }
}
