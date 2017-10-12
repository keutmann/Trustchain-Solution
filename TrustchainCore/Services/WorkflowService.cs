﻿using System;
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
        private ITrustDBService _trustDBService;
        private IServiceProvider _serviceProvider;

        public WorkflowService(ITrustDBService trustDBService, IServiceProvider serviceProvider)
        {
            _trustDBService = trustDBService;
            _serviceProvider = serviceProvider;
        }

        public T Create<T>(WorkflowContainer container = null) where T : IWorkflowContext
        {
            T instance = default(T);
            if(container == null || String.IsNullOrWhiteSpace(container.Data))
            {
                instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
                instance.State = WorkflowStatus.New.ToString();
                instance.Initialize(); // Initialize new workflow
                
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = _serviceProvider.GetService<IContractResolver>()
                };

                instance = JsonConvert.DeserializeObject<T>(container.Data, settings);
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

        public WorkflowContainer CreateWorkflowContainer(IWorkflowContext context)
        {
            var entity = new WorkflowContainer
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
