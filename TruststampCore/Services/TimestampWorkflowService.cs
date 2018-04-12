using Microsoft.Extensions.DependencyInjection;
using System;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TrustchainCore.Enumerations;
using TrustchainCore.Extensions;

namespace TruststampCore.Services
{
    public class TimestampWorkflowService : ITimestampWorkflowService
    {

        public IWorkflowService WorkflowService { get; } 
        private ITrustDBService _trustDBService;
        private ITimestampSynchronizationService _timestampSynchronizationService;

        public TimestampWorkflowService(IWorkflowService workflowService, ITrustDBService trustDBService, ITimestampSynchronizationService timestampSynchronizationService)
        {
            WorkflowService = workflowService;
            _trustDBService = trustDBService;
            _timestampSynchronizationService = timestampSynchronizationService;
        }

        public int CountCurrentProofs()
        {
            return _trustDBService.Proofs.Where(p => p.WorkflowID == _timestampSynchronizationService.CurrentWorkflowID).Count();
        }

        public void CreateAndExecute()
        {
            var oldID = _timestampSynchronizationService.CurrentWorkflowID;
            var wf = WorkflowService.Create<TimestampWorkflow>();
            wf.Container.State = WorkflowStatusType.Waiting.ToString();
            wf.Container.NextExecution = long.MaxValue;
            _timestampSynchronizationService.CurrentWorkflowID = WorkflowService.Save(wf);

            var oldWf = WorkflowService.Load<TimestampWorkflow>(oldID);
            oldWf.Container.State = WorkflowStatusType.Starting.ToString();
            oldWf.Container.NextExecution = DateTime.Now.ToUnixTime();
            WorkflowService.Save(oldWf);
        }

        public void EnsureTimestampScheduleWorkflow()
        {
            WorkflowService.EnsureWorkflow<TimestampScheduleWorkflow>();
        }

        public void EnsureTimestampWorkflow()
        {
            var timestampWorkflow = WorkflowService.EnsureWorkflow<TimestampWorkflow>();
            _timestampSynchronizationService.CurrentWorkflowID = timestampWorkflow.ID;
        }

    }
}
