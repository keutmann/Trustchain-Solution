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

namespace TrustchainCore.Services
{
    public class WorkflowService : IWorkflowService
    {
        public ITrustDBService DBService { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public WorkflowService(ITrustDBService trustDBService, IServiceProvider serviceProvider)
        {
            DBService = trustDBService;
            ServiceProvider = serviceProvider;
        }

        public T Create<T>(WorkflowEntity workflow = null) where T : IWorkflowContext
        {
            T instance = default(T);
            if(workflow == null || String.IsNullOrWhiteSpace(workflow.Data))
            {
                instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
                instance.State = WorkflowStatus.New.ToString();
                instance.Initialize(); // Initialize new workflow
                
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = ServiceProvider.GetService<IContractResolver>()
                };

                instance = JsonConvert.DeserializeObject<T>(workflow.Data, settings);
            }

            if (workflow != null)
            {
                instance.ID = workflow.ID;
                instance.State = workflow.State;
                instance.Tag = workflow.Tag;
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

        public int Save(IWorkflowContext context)
        {
            if(context.ID != 0)
            {
                var entity = DBService.Workflows.FirstOrDefault(w => w.ID == context.ID);
                if (entity != null)
                {
                    entity.State = context.State;
                    entity.Tag = context.Tag;
                    entity.Data = JsonConvert.SerializeObject(context);
                    DBService.DBContext.SaveChanges();
                    return context.ID; // Exit now!
                }
            }

            var workflow = CreateWorkflowEntity(context);
            DBService.DBContext.Workflows.Add(workflow);
            DBService.DBContext.SaveChanges();
            return workflow.ID;
        }

        public WorkflowEntity CreateWorkflowEntity(IWorkflowContext context)
        {
            var entity = new WorkflowEntity
            {
                Type = context.GetType().FullName,
                State = context.State,
                Tag = context.Tag,
                Data = JsonConvert.SerializeObject(context)
            };
            return entity;
        }

    }
}
