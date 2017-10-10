using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;

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
         

        public IWorkflowService WorkflowService { get; set; }

        public TimestampWorkflowService(IWorkflowService workflowService)
        {
            WorkflowService = workflowService;
        }

        public void CreateNextWorkflow()
        {
            var wf = WorkflowService.Create<TimestampWorkflow>();
            _currentWorkflowID = WorkflowService.Save(wf);
        }
    }
}
