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

        private IWorkflowService _workflowService;
        private ITrustDBService _trustDBService;
        private ITimestampSynchronizationService _timestampSynchronizationService;

        public TimestampWorkflowService(IWorkflowService workflowService, ITrustDBService trustDBService, ITimestampSynchronizationService timestampSynchronizationService)
        {
            _workflowService = workflowService;
            _trustDBService = trustDBService;
            _timestampSynchronizationService = timestampSynchronizationService;
        }

        public int CountCurrentProofs()
        {
            return _trustDBService.Proofs.Where(p => p.WorkflowID == _timestampSynchronizationService.CurrentWorkflowID).Count();
        }

        public void CreateNextTimestampWorkflow()
        {
            var wf = _workflowService.Create<TimestampWorkflow>();
            _timestampSynchronizationService.CurrentWorkflowID = _workflowService.Save(wf);
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
