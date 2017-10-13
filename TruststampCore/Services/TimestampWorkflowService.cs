using Microsoft.Extensions.DependencyInjection;
using System;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TrustchainCore.Enumerations;

namespace TruststampCore.Services
{
    public class TimestampWorkflowService : ITimestampWorkflowService
    {
        private int _currentWorkflowID = 0; // Field for atomic access
        public int CurrentWorkflowID
        {
            get
            {
                return _currentWorkflowID;
            }
        }
         

        
        private IServiceProvider _serviceProvider;

        public TimestampWorkflowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void CreateNextWorkflow()
        {
            var workflowService = _serviceProvider.GetRequiredService<IWorkflowService>();
            var wf = workflowService.Create<TimestampWorkflow>();
            _currentWorkflowID = workflowService.Save(wf);
        }

        public IList<TimestampWorkflow> GetRunningWorkflows()
        {
            var list = new List<TimestampWorkflow>();
            var trustDBService = _serviceProvider.GetRequiredService<ITrustDBService>();
            var workflowService = _serviceProvider.GetRequiredService<IWorkflowService>();
            var timestampWorkflowType = typeof(TimestampWorkflow).GetType().FullName;
            var containers = from p in trustDBService.Workflows
                             where p.ID != CurrentWorkflowID
                             && p.Type == timestampWorkflowType
                             && (p.State != WorkflowStatusType.Finished.ToString() || p.State != WorkflowStatusType.Failed.ToString())
                             select p;
            foreach (var container in containers)
            {
                list.Add(workflowService.Create<TimestampWorkflow>(container));
            }
            return list;
        }

        public void RunWorkflows()
        {
            int id = 0;
            var taskProcessor = new System.Timers.Timer { Interval = 1000 * 60 * 10 };
            taskProcessor.Elapsed += (sender, e) =>
            {
                var localID = id++;
                var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var timestampWorkflowService = scope.ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
                    timestampWorkflowService.CreateNextWorkflow(); // Ensure a workflow for proofs

                    var workflows = timestampWorkflowService.GetRunningWorkflows();

                    foreach (var workflow in workflows)
                    {
                        var task = workflow.Execute();
                        task.Wait(); // Timestamp workflow need to be syncron because of blockchain TX output
                    }
                }
            };
            taskProcessor.Start();
        }
    }
}
