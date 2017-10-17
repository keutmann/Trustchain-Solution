using System.Linq;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Interfaces;
using TrustchainCore.Workflows;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class RunTimestampStep : WorkflowStep, IRunTimestampStep
    {
        private ITimestampWorkflowService _timestampWorkflowService;
        private IConfiguration _configuration;

        public RunTimestampStep(ITimestampWorkflowService timestampWorkflowService, IConfiguration configuration)
        {
            _timestampWorkflowService = timestampWorkflowService;
            _configuration = configuration;
        }

        public override void Execute()
        {
            if(_timestampWorkflowService.CountCurrentProofs() > 0)
            {
                _timestampWorkflowService.CreateNextTimestampWorkflow();
            }

            // Rerun this step after 10 min, never to exit
            Context.RunStepAgain(_configuration.GetValue<int>("TimestampInterval", 60 * 10));
        }
    }
}
