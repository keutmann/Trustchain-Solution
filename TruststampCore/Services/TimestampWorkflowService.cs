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


        private IWorkflowService _workflowService;
        private IServiceProvider _serviceProvider;

        public TimestampWorkflowService(IWorkflowService workflowService, IServiceProvider serviceProvider)
        {
            _workflowService = workflowService;
            _serviceProvider = serviceProvider;
        }

        public void CreateNextWorkflow()
        {
            var workflowService = _serviceProvider.GetRequiredService<IWorkflowService>();
            var wf = workflowService.Create<TimestampWorkflow>();
            _currentWorkflowID = workflowService.Save(wf);
        }


        public void EnsureTimestampWorkflow()
        {
            var timestampWorkflowContainer = _workflowService.Workflows.FirstOrDefault(p => p.Type == typeof(TimestampScheduleWorkflow).FullName
                                             && (p.State == WorkflowStatusType.New.ToString() 
                                             || p.State == WorkflowStatusType.Running.ToString()));
                                             
            if(timestampWorkflowContainer == null)
            {
                var wf = _workflowService.Create<TimestampScheduleWorkflow>();
                _workflowService.Save(wf);
            }
        }

    }
}
