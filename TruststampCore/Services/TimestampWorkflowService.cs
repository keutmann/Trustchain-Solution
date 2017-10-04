using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Services;

namespace TruststampCore.Services
{
    public class TimestampWorkflowService
    {
        public IWorkflowService WorkflowService { get; set; }

        public TimestampWorkflowService(IWorkflowService workflowService)
        {
            WorkflowService = workflowService;
        }
    }
}
