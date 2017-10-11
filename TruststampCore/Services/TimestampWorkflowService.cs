using System;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
