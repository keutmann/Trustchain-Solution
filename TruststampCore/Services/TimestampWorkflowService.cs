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

        public void InitiliazeTimestampSynchronizationService()
        {
            var timestampWorkflow = _workflowService.GetRunningWorkflow<TimestampWorkflow>();
            _timestampSynchronizationService.CurrentWorkflowID = timestampWorkflow.ID;
        }

        public void EnsureTimestampScheduleWorkflow()
        {
            _workflowService.EnsureWorkflow<TimestampScheduleWorkflow>();
        }

        public void EnsureTimestampWorkflow()
        {
            var timestampWorkflow = _workflowService.EnsureWorkflow<TimestampWorkflow>();
            _timestampSynchronizationService.CurrentWorkflowID = timestampWorkflow.ID;
        }

    }
}
